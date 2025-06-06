using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Pawesome.Interfaces;
using Pawesome.Models.Configuration;
using Pawesome.Models.Entities;
using Pawesome.Models.enums;
using Pawesome.Models.Enums;
using Pawesome.Models.ViewModels.Payment;
using Pawesome.Repositories;
using Stripe;
using Stripe.Checkout;

namespace Pawesome.Controllers
{
    /// <summary>
    /// Controller responsible for handling payment-related operations
    /// </summary>
    [Authorize]
    public class PaymentController : Controller
    {
        private readonly IPaymentService _paymentService;
        private readonly IAdvertService _advertService;
        private readonly IBookingService _bookingService;
        private readonly UserManager<User> _userManager;
        private readonly StripeSettings _stripeSettings;
        private readonly ILogger<PaymentController> _logger;
        private readonly IPaymentRepository _paymentRepository;
        private readonly IUserRepository _userRepository;
        private readonly IStripeBalanceService _balanceService;

        /// <summary>
        /// Initializes a new instance of the PaymentController
        /// </summary>
        /// <param name="paymentService">Service for managing payment operations</param>
        /// <param name="advertService">Service for managing advertisements</param>
        /// <param name="bookingService">Service for managing bookings</param>
        /// <param name="userManager">User manager for user operations</param>
        /// <param name="stripeSettings">Stripe configuration settings</param>
        /// <param name="logger">Logger for recording diagnostic information</param>
        /// <param name="paymentRepository"></param>
        /// <param name="userRepository"></param>
        /// <param name="balanceService"></param>
        public PaymentController(
            IPaymentService paymentService,
            IAdvertService advertService,
            IBookingService bookingService,
            UserManager<User> userManager,
            IOptions<StripeSettings> stripeSettings,
            ILogger<PaymentController> logger,
            IPaymentRepository paymentRepository,
            IUserRepository userRepository,
            IStripeBalanceService balanceService
        )
        {
            _paymentService = paymentService;
            _advertService = advertService;
            _bookingService = bookingService;
            _userManager = userManager;
            _stripeSettings = stripeSettings.Value;
            _logger = logger;
            _paymentRepository = paymentRepository;
            _userRepository = userRepository;
            _balanceService = balanceService;
        }

        /// <summary>
        /// Displays the checkout page for a specific booking
        /// </summary>
        /// <param name="bookingId">ID of the booking to process payment for</param>
        /// <returns>The checkout view or an error [HttpGet] </returns>
        [HttpGet]
        [HttpPost]
        public async Task<IActionResult> Checkout(int bookingId)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return Challenge();

            var booking = await _bookingService.GetBookingByIdAsync(bookingId);
            if (booking == null)
            {
                return NotFound("La réservation demandée n'existe pas.");
            }

            if (booking.PetSitterUserId != user.Id && booking.BookerUserId != user.Id)
            {
                return Forbid();
            }

            var advert = await _advertService.GetAdvertByIdAsync(booking.AdvertId);
            if (advert == null)
            {
                return NotFound("L'annonce associée à cette réservation n'existe plus.");
            }

            try
            {
                var viewModel = new CheckoutViewModel
                {
                    BookingId = bookingId,
                    AdvertId = booking.AdvertId,
                    Amount = booking.Amount,
                    StripePublishableKey = _stripeSettings.PublishableKey
                };

                return View(viewModel);
            }
            catch (Exception)
            {
                return RedirectToAction("Error", "Home",
                    new { message = "Une erreur s'est produite lors de la préparation du paiement." });
            }
        }

        /// <summary>
        /// Creates a Stripe checkout session for payment processing
        /// </summary>
        /// <param name="request">Request containing booking and advert information</param>
        /// <returns>JSON response with session ID or error details</returns>
        [HttpPost]
        public async Task<IActionResult> CreateCheckoutSession([FromBody] CheckoutSessionRequest request)
        {
            try
            {
                if (request.BookingId <= 0)
                {
                    return BadRequest(new { error = "Données de réservation invalides" });
                }

                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                    return Challenge();

                var booking = await _bookingService.GetBookingByIdAsync(request.BookingId);
                if (booking == null)
                    return NotFound(new { error = "Réservation non trouvée" });

                if (booking.PetSitterUserId != user.Id && booking.BookerUserId != user.Id)
                    return Forbid();

                var advert = await _advertService.GetAdvertByIdAsync(booking.AdvertId);
                if (advert == null)
                    return NotFound(new { error = "Annonce non trouvée" });

                var options = new SessionCreateOptions
                {
                    PaymentMethodTypes = new List<string> { "card" },
                    LineItems = new List<SessionLineItemOptions>
                    {
                        new SessionLineItemOptions
                        {
                            PriceData = new SessionLineItemPriceDataOptions
                            {
                                UnitAmount = (long)(booking.Amount * 100),
                                Currency = "eur",
                                ProductData = new SessionLineItemPriceDataProductDataOptions
                                {
                                    Name = $"Réservation pour {advert.Owner.FullName}",
                                    Description = $"Du {booking.StartDate:dd/MM/yyyy} au {booking.EndDate:dd/MM/yyyy}"
                                }
                            },
                            Quantity = 1
                        }
                    },
                    Mode = "payment",
                    SuccessUrl =
                        $"{Request.Scheme}://{Request.Host}/Payment/Success?session_id={{CHECKOUT_SESSION_ID}}",
                    CancelUrl = $"{Request.Scheme}://{Request.Host}/Payment/Cancel",
                    CustomerEmail = user.Email,
                    ClientReferenceId = booking.Id.ToString(),
                    PaymentIntentData = new SessionPaymentIntentDataOptions
                    {
                        CaptureMethod = "manual"
                    }
                };

                var service = new SessionService();
                var session = await service.CreateAsync(options);

                var payment = new Payment
                {
                    BookingId = booking.Id,
                    Amount = booking.Amount,
                    PaymentIntentId = session.PaymentIntentId,
                    SessionId = session.Id,
                    Status = PaymentStatus.Authorized,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    Booking = default!
                };

                await _paymentRepository.CreatePaymentAsync(payment);

                if (booking.PetSitterUserId == user.Id)
                {
                    await _advertService.UpdateAdvertStatusAsync(booking.AdvertId, AdvertStatus.Pending);
                    await _bookingService.UpdateBookingStatusAsync(booking.Id, BookingStatus.Accepted);
                }

                return Json(new { id = session.Id });
            }
            catch (StripeException e)
            {
                return BadRequest(new { error = $"Erreur de paiement: {e.Message}" });
            }
            catch (Exception)
            {
                return BadRequest(new { error = "Une erreur inattendue s'est produite" });
            }
        }

        /// <summary>
        /// Handles successful payment processing and redirects to success page
        /// </summary>
        /// <param name="session_id">The Stripe session ID for the completed payment</param>
        /// <returns>Success view with payment details or redirect with error message</returns>
        [HttpGet]
        public async Task<IActionResult> Success(string session_id)
        {
            try
            {
                var payment = await _paymentRepository.GetPaymentBySessionIdAsync(session_id);

                if (payment == null)
                {
                    TempData["ErrorMessage"] = "Paiement non trouvé.";
                    return RedirectToAction("Index", "Home");
                }

                payment = await _paymentRepository.UpdatePaymentStatusAsync(session_id, PaymentStatus.Captured);

                if (payment == null)
                {
                    TempData["ErrorMessage"] = "Erreur lors de la mise à jour du paiement.";
                    return RedirectToAction("Index", "Home");
                }

                var model = new SuccessViewModel
                {
                    BookingId = payment.BookingId,
                    Amount = payment.Amount,
                    PaymentDate = DateTime.UtcNow,
                    IsAutoAccepted = true
                };

                TempData["SuccessMessage"] = "Paiement réussi ! Votre réservation a été créée avec succès.";
                return RedirectToAction("Details", "Booking", new { id = payment.BookingId });
            }
            catch (Exception)
            {
                TempData["ErrorMessage"] = "Une erreur est survenue lors de la finalisation du paiement.";
                return RedirectToAction("Index", "Home");
            }
        }

        /// <summary>
        /// Handles payment cancellation
        /// </summary>
        /// <returns>Cancellation view</returns>
        [HttpGet]
        public IActionResult Cancel()
        {
            return View();
        }

        /// <summary>
        /// Displays the payment history for the current user
        /// </summary>
        /// <returns>Payment history view with list of user's payments</returns>
        [HttpGet]
        public async Task<IActionResult> History()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return Challenge();

            var paymentHistory = await _paymentService.GetUserPaymentsForHistoryAsync(user.Id);
            return View(paymentHistory);
        }
    }

    /// <summary>
    /// Request model for creating a checkout session
    /// </summary>
    public class CheckoutSessionRequest
    {
        /// <summary>
        /// ID of the booking to be paid for
        /// </summary>
        public int BookingId { get; set; }

        /// <summary>
        /// ID of the advertisement related to the booking
        /// </summary>
        public int AdvertId { get; set; }
    }
}