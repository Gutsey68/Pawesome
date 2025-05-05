using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Pawesome.Interfaces;
using Pawesome.Models.Configuration;
using Pawesome.Models.Entities;
using Pawesome.Models.ViewModels.Payment;
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
        private readonly UserManager<User> _userManager;
        private readonly IMapper _mapper;

        /// <summary>
        /// Initializes a new instance of the <see cref="PaymentController"/> class
        /// </summary>
        /// <param name="stripeSettings">Stripe configuration settings</param>
        /// <param name="paymentService">Service for managing payments</param>
        /// <param name="advertService">Service for managing advertisements</param>
        /// <param name="userManager">User management service</param>
        /// <param name="mapper">AutoMapper instance for object mapping</param>
        public PaymentController(
            IOptions<StripeSettings> stripeSettings,
            IPaymentService paymentService,
            IAdvertService advertService,
            UserManager<User> userManager,
            IMapper mapper)
        {
            _stripeSettings = stripeSettings;
            _paymentService = paymentService;
            _advertService = advertService;
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

            await _paymentService.CreatePaymentAsync(user.Id, advertId, session.Id);

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
    }
}