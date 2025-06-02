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

        /// <summary>
        /// Initializes a new instance of the PaymentController
        /// </summary>
        /// <param name="paymentService">Service for managing payment operations</param>
        /// <param name="advertService">Service for managing advertisements</param>
        /// <param name="bookingService">Service for managing bookings</param>
        /// <param name="userManager">User manager for user operations</param>
        /// <param name="stripeSettings">Stripe configuration settings</param>
        /// <param name="logger">Logger for recording diagnostic information</param>
        public PaymentController(
            IPaymentService paymentService,
            IAdvertService advertService,
            IBookingService bookingService,
            UserManager<User> userManager,
            IOptions<StripeSettings> stripeSettings,
            ILogger<PaymentController> logger)
        {
            _paymentService = paymentService;
            _advertService = advertService;
            _bookingService = bookingService;
            _userManager = userManager;
            _stripeSettings = stripeSettings.Value;
            _logger = logger;
        }

        /// <summary>
        /// Displays the checkout page for a specific booking
        /// </summary>
        /// <param name="bookingId">ID of the booking to process payment for</param>
        /// <returns>The checkout view or an error [HttpGet]
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

            if (booking.PetSitterUserId != user.Id && booking.BookerUserId != user.Id )
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
                return RedirectToAction("Error", "Home", new { message = "Une erreur s'est produite lors de la préparation du paiement." });
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
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                    return Challenge();

                var booking = await _bookingService.GetBookingByIdAsync(request.BookingId);
                if (booking == null)
                    return NotFound(new { error = "Réservation non trouvée" });

                if (booking.PetSitterUserId != user.Id && booking.BookerUserId != user.Id )
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
                    SuccessUrl = $"{Request.Scheme}://{Request.Host}/Payment/Success?session_id={{CHECKOUT_SESSION_ID}}",
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

                if (booking.PetSitterUserId == user.Id)
                {
                    _logger.LogWarning("Ca passe la");
                    await _advertService.UpdateAdvertStatusAsync(booking.AdvertId, AdvertStatus.FullyBooked);
                    await _bookingService.UpdateBookingStatusAsync(booking.Id, BookingStatus.Completed);
                }
                _logger.LogWarning("Ca passe pas la");

                return Json(new { id = session.Id });
            }
            catch (StripeException e)
            {
                _logger.LogError(e, "Erreur Stripe lors de la création de la session de paiement");
                return BadRequest(new { error = $"Erreur de paiement: {e.Message}" });
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Erreur lors de la création de la session de paiement");
                return BadRequest(new { error = "Une erreur inattendue s'est produite" });
            }
        }

        /// <summary>
        /// Handles successful payment processing and redirects to success page
        /// </summary>
        /// <param name="sessionId">The Stripe session ID for the completed payment</param>
        /// <returns>Success view with payment details or redirect with error message</returns>
        [HttpGet]
        public async Task<IActionResult> Success(string sessionId)
        {
            if (string.IsNullOrEmpty(sessionId))
            {
                return RedirectToAction("Index", "Home");
            }
    
            var payment = await _paymentService.CompletePaymentAsync(sessionId);
    
            if (payment == null)
            {
                TempData["ErrorMessage"] = "Impossible de récupérer les détails du paiement.";
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
