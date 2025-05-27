using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Pawesome.Interfaces;
using Pawesome.Models.Configuration;
using Pawesome.Models.Entities;
using Pawesome.Models.Enums;
using Pawesome.Models.ViewModels.Payment;
using Stripe;
using Stripe.Checkout;

namespace Pawesome.Controllers
{
    /// <summary>
    /// Controller responsible for handling payment-related operations
    /// </summary>
    public class PaymentController : Controller
    {
        private readonly IOptions<StripeSettings> _stripeSettings;
        private readonly IPaymentService _paymentService;
        private readonly IAdvertService _advertService;
        private readonly IPaymentRepository _paymentRepository;
        private readonly UserManager<User> _userManager;
        private readonly IMapper _mapper;

        /// <summary>
        /// Initializes a new instance of the <see cref="PaymentController"/> class
        /// </summary>
        /// <param name="stripeSettings">Stripe configuration settings</param>
        /// <param name="paymentService">Service for managing payments</param>
        /// <param name="advertService">Service for managing advertisements</param>
        /// <param name="paymentRepository">Repository for payment operations</param>
        /// <param name="userManager">User management service</param>
        /// <param name="mapper">AutoMapper instance for object mapping</param>
        public PaymentController(
            IOptions<StripeSettings> stripeSettings,
            IPaymentService paymentService,
            IAdvertService advertService,
            IPaymentRepository paymentRepository,
            UserManager<User> userManager,
            IMapper mapper)
        {
            _stripeSettings = stripeSettings;
            _paymentService = paymentService;
            _advertService = advertService;
            _paymentRepository = paymentRepository;
            _userManager = userManager;
            _mapper = mapper;
        }

        /// <summary>
        /// Displays the checkout page for an advertisement
        /// </summary>
        /// <param name="advertId">The ID of the advertisement to checkout</param>
        /// <returns>The checkout view with advert details</returns>
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Checkout(int advertId)
        {
            var advert = await _advertService.GetAdvertByIdAsync(advertId);

            if (advert == null)
            {
                return NotFound();
            }

            var viewModel = _mapper.Map<CheckoutViewModel>(advert);

            viewModel.StripePublishableKey = _stripeSettings.Value.PublishableKey;

            return View(viewModel);
        }

        /// <summary>
        /// Creates a Stripe checkout session for payment processing
        /// </summary>
        /// <param name="advertId">The ID of the advertisement to pay for</param>
        /// <returns>Redirect to Stripe checkout page</returns>
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> CreateCheckoutSession(int advertId)
        {
            StripeConfiguration.ApiKey = _stripeSettings.Value.SecretKey;

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Challenge();
            }

            var advert = await _advertService.GetAdvertByIdAsync(advertId);
            if (advert == null)
            {
                return NotFound();
            }
            
            var existingPayments = await _paymentRepository.GetPaymentsByUserAndAdvertAsync(user.Id, advertId);
            var existingValidPayment = existingPayments.FirstOrDefault(p => p.Status != PaymentStatus.Failed);
            
            if (existingValidPayment != null)
            {
                return RedirectToAction("Success", new { sessionId = existingValidPayment.SessionId });
            }

            if (TempData["LastSessionId"] != null &&
                !string.IsNullOrEmpty(TempData["LastSessionId"]?.ToString()) &&
                TempData["LastAdvertId"] != null &&
                (int)(TempData["LastAdvertId"] ?? throw new InvalidOperationException()) == advertId)
            {
                string lastSessionId = TempData["LastSessionId"]?.ToString() ?? throw new InvalidOperationException();
                var sessionService = new SessionService();
                var existingSession = await sessionService.GetAsync(lastSessionId);
                if (existingSession != null && !string.IsNullOrEmpty(existingSession.Url))
                {
                    return Redirect(existingSession.Url);
                }
            }

            var options = new SessionCreateOptions
            {
                PaymentMethodTypes = new List<string> { "card" },
                LineItems = new List<SessionLineItemOptions>
                {
                    new SessionLineItemOptions
                    {
                        PriceData = new SessionLineItemPriceDataOptions
                        {
                            Currency = "eur",
                            ProductData = new SessionLineItemPriceDataProductDataOptions
                            {
                                Name = advert.IsPetSitter
                                    ? "Réservation de services de pet sitting"
                                    : "Réservation d'un pet sitter",
                                Description = $"Du {advert.StartDate:dd/MM/yyyy} au {advert.EndDate:dd/MM/yyyy}"
                            },
                            UnitAmount = (long)(advert.Amount * 100),
                        },
                        Quantity = 1,
                    },
                },
                Mode = "payment",
                Locale = "fr",
                SuccessUrl = Url.Action("Success", "Payment", null, Request.Scheme) + "?session_id={CHECKOUT_SESSION_ID}",
                CancelUrl = Url.Action("Cancel", "Payment", null, Request.Scheme),
                CustomerEmail = user.Email
            };

            var service = new SessionService();
            var session = await service.CreateAsync(options);
            
            TempData["LastSessionId"] = session.Id;
            TempData["LastAdvertId"] = advertId;

            try
            {
                await _paymentService.CreatePaymentAsync(user.Id, advertId, session.Id);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur lors de la création du paiement : {ex.Message}");
            }

            return Redirect(session.Url);
        }

        /// <summary>
        /// Handles successful payment completion
        /// </summary>
        /// <param name="sessionId">The Stripe session ID of the completed payment</param>
        /// <returns>Success view with payment confirmation details</returns>
        [Authorize]
        public async Task<IActionResult> Success(string sessionId)
        {
            if (string.IsNullOrEmpty(sessionId))
            {
                return RedirectToAction("Index", "Home");
            }

            var payment = await _paymentService.CompletePaymentAsync(sessionId);

            if (payment == null)
            {
                return NotFound();
            }

            ViewBag.SessionId = sessionId;
            ViewBag.PaymentId = payment.Id;

            return View();
        }

        /// <summary>
        /// Handles canceled a payment process
        /// </summary>
        /// <returns>Cancellation view</returns>
        public IActionResult Cancel()
        {
            return View();
        }

        /// <summary>
        /// Displays payment history for the current user
        /// </summary>
        /// <returns>View with the user's payment history</returns>
        [Authorize]
        public async Task<IActionResult> History()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Challenge();
            }

            var viewModels = await _paymentService.GetUserPaymentsForHistoryAsync(user.Id);

            return View(viewModels);
        }

        /// <summary>
        /// Checks if a payment already exists for the current user and a specific advertisement
        /// </summary>
        /// <param name="advertId">The ID of the advertisement to check payments for</param>
        /// <returns>JSON result indicating whether a valid payment exists</returns>
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> CheckPaymentExists(int advertId)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Challenge();
            }

            var payments = await _paymentRepository.GetPaymentsByUserAndAdvertAsync(user.Id, advertId);
            var exists = payments.Any(p => p.Status != PaymentStatus.Failed);

            return Json(new { exists });
        }
    }
}