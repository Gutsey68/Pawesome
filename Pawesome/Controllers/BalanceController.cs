using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Pawesome.Interfaces;
using Pawesome.Models.Entities;
using Pawesome.Models.ViewModels.Balance;

namespace Pawesome.Controllers
{
    /// <summary>
    /// Controller responsible for handling stripe balance operations and account setup
    /// </summary>
    [Authorize]
    public class BalanceController : Controller
    {
        private readonly IStripeBalanceService _balanceService;
        private readonly UserManager<User> _userManager;
        private readonly IUserRepository _userRepository;
        private readonly ILogger<BalanceController> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="BalanceController"/> class.
        /// </summary>
        /// <param name="balanceService">Service handling stripe balance operations</param>
        /// <param name="userManager">Identity user manager</param>
        /// <param name="userRepository"></param>
        /// <param name="logger">Logger instance</param>
        public BalanceController(
            IStripeBalanceService balanceService,
            UserManager<User> userManager,
            IUserRepository userRepository,
            ILogger<BalanceController> logger)
        {
            _balanceService = balanceService;
            _userManager = userManager;
            _userRepository = userRepository;
            _logger = logger;
        }

        /// <summary>
        /// Displays the user's balance and payout history
        /// </summary>
        /// <returns>Balance view with account information</returns>
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return Challenge();

            var isOnboardingCompleted = await _balanceService.IsOnboardingCompletedAsync(user.Id);
            
            if (!isOnboardingCompleted)
            {
                return View("OnboardingRequired");
            }

            var balance = await _balanceService.GetUserBalanceAsync(user.Id);
            var payoutHistory = await _balanceService.GetUserPayoutHistoryAsync(user.Id);

            var viewModel = new BalanceViewModel
            {
                AvailableBalance = balance,
                IsOnboardingCompleted = isOnboardingCompleted,
                PayoutHistory = payoutHistory
            };

            return View(viewModel);
        }

        /// <summary>
        /// Initiates the Stripe Connect account setup process
        /// </summary>
        /// <returns>Redirects to Stripe's onboarding flow or back to balance page if already completed</returns>
        [HttpGet]
        public async Task<IActionResult> SetupAccount()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return Challenge();

            var isAlreadyCompleted = await _balanceService.IsOnboardingCompletedAsync(user.Id);
            if (isAlreadyCompleted)
            {
                return RedirectToAction(nameof(Index));
            }

            var returnUrl = Url.Action(nameof(OnboardingReturn), "Balance", null, Request.Scheme);
            if (returnUrl != null)
            {
                var onboardingUrl = await _balanceService.CreateOnboardingLinkAsync(user.Id, returnUrl);
                
                var onboardingResult = await _userRepository.SetStripeOnboardingCompletedAsync(user.Id);
                if (!onboardingResult)
                {
                    _logger.LogWarning($"Impossible de mettre à jour le statut d'onboarding pour l'utilisateur {user.Id}");
                }

                return Redirect(onboardingUrl);
            }
            
            return View("OnboardingRequired");
        }

        /// <summary>
        /// Handles the return from Stripe's onboarding flow
        /// </summary>
        /// <returns>Redirects to the balance page with status message</returns>
        [HttpGet]
        public async Task<IActionResult> OnboardingReturn()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return Challenge();

            await _balanceService.CheckOnboardingStatusAsync(user.Id);

            return RedirectToAction(nameof(Index));
        }

        /// <summary>
        /// Creates a payout request to transfer funds from Stripe to user's bank account
        /// </summary>
        /// <param name="amount">Amount to withdraw in euros</param>
        /// <returns>Redirects to the balance page with success or error message</returns>
        [HttpPost]
        public async Task<IActionResult> CreatePayout(decimal amount)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return Challenge();

            if (amount <= 0)
            {
                TempData["ErrorMessage"] = "Le montant du retrait doit être supérieur à zéro.";
                return RedirectToAction(nameof(Index));
            }

            var balance = await _balanceService.GetUserBalanceAsync(user.Id);
            if (amount > balance)
            {
                TempData["ErrorMessage"] = "Le montant du retrait ne peut pas dépasser votre solde disponible.";
                return RedirectToAction(nameof(Index));
            }

            try
            {
                await _balanceService.CreatePayoutLinkAsync(user.Id, amount);
                TempData["SuccessMessage"] = "Votre demande de retrait a été traitée avec succès.";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la création du retrait pour l'utilisateur {UserId}", user.Id);
                TempData["ErrorMessage"] = "Une erreur s'est produite lors de la demande de retrait.";
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
