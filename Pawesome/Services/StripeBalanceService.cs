using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Pawesome.Interfaces;
using Pawesome.Models.ViewModels.Balance;
using Stripe;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Pawesome.Models.Entities;

namespace Pawesome.Services
{
    /// <summary>
    /// Service for handling Stripe balance operations
    /// </summary>
    public class StripeBalanceService : IStripeBalanceService
    {
        private readonly IUserRepository _userRepository;
        private readonly ILogger<StripeBalanceService> _logger;
        private readonly INotificationService _notificationService;

        /// <summary>
        /// Initializes a new instance of the <see cref="StripeBalanceService"/> class
        /// </summary>
        /// <param name="userRepository">The user repository</param>
        /// <param name="notificationService">The notification service</param>
        /// <param name="logger">The logger</param>
        public StripeBalanceService(
            IUserRepository userRepository,
            INotificationService notificationService,
            ILogger<StripeBalanceService> logger)
        {
            _userRepository = userRepository;
            _logger = logger;
            _notificationService = notificationService;
        }

        /// <summary>
        /// Gets the available balance for a user from their Stripe account
        /// </summary>
        /// <param name="userId">The user ID</param>
        /// <returns>The available balance in euros</returns>
        public async Task<decimal> GetUserBalanceAsync(int userId)
        {
            try
            {
                var user = await _userRepository.GetByIdAsync(userId);
                if (user == null || string.IsNullOrEmpty(user.StripeAccountId) || !user.IsStripeOnboardingCompleted)
                {
                    _logger.LogWarning(
                        "Impossible de récupérer le solde: utilisateur {UserId} non trouvé ou compte Stripe non configuré",
                        userId);
                    return 0;
                }

                var balanceService = new BalanceService();
                var requestOptions = new RequestOptions
                {
                    StripeAccount = user.StripeAccountId
                };

                var balance = await balanceService.GetAsync(requestOptions);

                var availableBalance = balance.Available
                    .FirstOrDefault(b => b.Currency == "eur")?.Amount ?? 0;

                decimal balanceInEuros = availableBalance / 100m;

                _logger.LogInformation("Solde Stripe récupéré pour l'utilisateur {UserId}: {Balance}€", userId,
                    balanceInEuros);

                return balanceInEuros;
            }
            catch (StripeException ex)
            {
                _logger.LogError(ex, "Erreur Stripe lors de la récupération du solde pour l'utilisateur {UserId}",
                    userId);
                return 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la récupération du solde Stripe pour l'utilisateur {UserId}",
                    userId);
                return 0;
            }
        }

        /// <summary>
        /// Checks if a user has completed the Stripe onboarding process
        /// </summary>
        /// <param name="userId">The user ID</param>
        /// <returns>True if onboarding is completed, false otherwise</returns>
        public async Task<bool> IsOnboardingCompletedAsync(int userId)
        {
            try
            {
                var user = await _userRepository.GetByIdAsync(userId);
                if (user == null || !user.IsStripeOnboardingCompleted)
                {
                    return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Erreur lors de la vérification du statut d'intégration pour l'utilisateur {UserId}", userId);
                return false;
            }
        }

        /// <summary>
        /// Creates a Stripe onboarding link for a user
        /// </summary>
        /// <param name="userId">The user ID</param>
        /// <param name="returnUrl">The URL to return to after onboarding</param>
        /// <returns>The onboarding link URL</returns>
        /// <exception cref="InvalidOperationException">Thrown when the user is not found</exception>
        public async Task<string> CreateOnboardingLinkAsync(int userId, string returnUrl)
        {
            try
            {
                var user = await _userRepository.GetByIdAsync(userId);
                if (user == null)
                {
                    throw new InvalidOperationException("Utilisateur non trouvé");
                }

                if (string.IsNullOrEmpty(user.StripeAccountId))
                {
                    var accountService = new AccountService();
                    var account = await accountService.CreateAsync(new AccountCreateOptions
                    {
                        Type = "express",
                        Country = "FR",
                        Email = user.Email,
                        BusinessType = "individual",

                        Capabilities = new AccountCapabilitiesOptions
                        {
                            CardPayments = new AccountCapabilitiesCardPaymentsOptions { Requested = true },
                            Transfers = new AccountCapabilitiesTransfersOptions { Requested = true }
                        },

                        BusinessProfile = new AccountBusinessProfileOptions
                        {
                            ProductDescription = "Services de garde d’animaux proposés via la plateforme Pawesome",
                        }
                    });

                    user.StripeAccountId = account.Id;
                    await _userRepository.UpdateAsync(user);
                }

                var accountLinkService = new AccountLinkService();
                var accountLink = await accountLinkService.CreateAsync(new AccountLinkCreateOptions
                {
                    Account = user.StripeAccountId,
                    RefreshUrl = returnUrl,
                    ReturnUrl = returnUrl,
                    Type = "account_onboarding"
                });

                return accountLink.Url;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la création du lien d'intégration pour l'utilisateur {UserId}",
                    userId);
                throw;
            }
        }


        /// <summary>
        /// Checks the onboarding status of a user and updates the database if completed
        /// </summary>
        /// <param name="userId">The user ID</param>
        /// <returns>True if onboarding is completed, false otherwise</returns>
        public async Task<bool> CheckOnboardingStatusAsync(int userId)
        {
            try
            {
                var user = await _userRepository.GetByIdAsync(userId);
                if (user == null || string.IsNullOrEmpty(user.StripeAccountId))
                {
                    return false;
                }

                var accountService = new AccountService();
                var account = await accountService.GetAsync(user.StripeAccountId);

                bool isCompleted = account.DetailsSubmitted == true &&
                                   account.PayoutsEnabled == true &&
                                   account.ChargesEnabled == true;

                if (isCompleted && !user.IsStripeOnboardingCompleted)
                {
                    user.IsStripeOnboardingCompleted = true;
                    user.IsOnboardingCompleted = true;
                    await _userRepository.UpdateAsync(user);
                    await _userRepository.SaveChangesAsync();

                    await _notificationService.CreateNotificationAsync(new Models.Entities.Notification
                    {
                        UserId = userId,
                        Title = "Configuration Stripe terminée",
                        Message =
                            "Votre compte Stripe a été configuré avec succès. Vous pouvez maintenant recevoir des paiements.",
                        Type = Models.Enums.NotificationType.AccountUpdate,
                        IsRead = false,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow,
                        User = user
                    });
                }

                return isCompleted;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Erreur lors de la vérification du statut d'intégration pour l'utilisateur {UserId}", userId);
                return false;
            }
        }

        /// <summary>
        /// Creates a payout request for a user's Stripe account
        /// </summary>
        /// <param name="userId">The user ID</param>
        /// <param name="amount">The amount to withdraw in euros</param>
        /// <returns>The payout ID</returns>
        /// <exception cref="InvalidOperationException">Thrown when the user doesn't have a configured Stripe account or the amount is invalid</exception>
        public async Task<string> CreatePayoutLinkAsync(int userId, decimal amount)
        {
            try
            {
                var user = await _userRepository.GetByIdAsync(userId);
                if (user == null || string.IsNullOrEmpty(user.StripeAccountId) || !user.IsOnboardingCompleted)
                {
                    throw new InvalidOperationException("L'utilisateur ne possède pas de compte Stripe configuré");
                }

                if (amount <= 0)
                {
                    throw new InvalidOperationException("Le montant du retrait doit être supérieur à zéro");
                }

                var amountInCents = Convert.ToInt64(amount * 100);

                var payoutService = new PayoutService();
                var payoutOptions = new PayoutCreateOptions
                {
                    Amount = amountInCents,
                    Currency = "eur",
                    Description = $"Retrait demandé le {DateTime.Now.ToString("dd/MM/yyyy")}"
                };

                var requestOptions = new RequestOptions
                {
                    StripeAccount = user.StripeAccountId
                };

                var payout = await payoutService.CreateAsync(payoutOptions, requestOptions);

                return payout.Id;
            }
            catch (StripeException ex)
            {
                _logger.LogError(ex, "Erreur Stripe lors de la création d'un versement pour l'utilisateur {UserId}",
                    userId);
                throw new InvalidOperationException($"Erreur lors de la demande de retrait : {ex.Message}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la création d'un versement pour l'utilisateur {UserId}", userId);
                throw;
            }
        }

        /// <summary>
        /// Gets the payout history for a user's Stripe account
        /// </summary>
        /// <param name="userId">The user ID</param>
        /// <returns>A list of payout history view models</returns>
        public async Task<List<PayoutHistoryViewModel>> GetUserPayoutHistoryAsync(int userId)
        {
            try
            {
                var user = await _userRepository.GetByIdAsync(userId);
                if (user == null || string.IsNullOrEmpty(user.StripeAccountId) || !user.IsOnboardingCompleted)
                {
                    return new List<PayoutHistoryViewModel>();
                }

                var payoutService = new PayoutService();
                var listOptions = new PayoutListOptions
                {
                    Limit = 100
                };

                var requestOptions = new RequestOptions
                {
                    StripeAccount = user.StripeAccountId
                };

                var payouts = await payoutService.ListAsync(listOptions, requestOptions);

                var payoutHistory = new List<PayoutHistoryViewModel>();

                foreach (var payout in payouts)
                {
                    payoutHistory.Add(new PayoutHistoryViewModel
                    {
                        Id = payout.Id,
                        Amount = payout.Amount / 100m,
                        Status = payout.Status,
                        CreatedAt = payout.Created,
                        ArrivalDate = payout.ArrivalDate
                    });
                }

                return payoutHistory;
            }
            catch (StripeException ex)
            {
                _logger.LogError(ex,
                    "Erreur Stripe lors de la récupération de l'historique des versements pour l'utilisateur {UserId}",
                    userId);
                return new List<PayoutHistoryViewModel>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Erreur lors de la récupération de l'historique des versements pour l'utilisateur {UserId}",
                    userId);
                return new List<PayoutHistoryViewModel>();
            }
        }

        /// <summary>
        /// Updates the user's local balance from the Stripe balance
        /// </summary>
        /// <param name="userId">The user's identifier</param>
        /// <returns>The updated balance</returns>
        public async Task<decimal> UpdateLocalBalanceFromStripeAsync(int userId)
        {
            try
            {
                var user = await _userRepository.GetByIdAsync(userId);
                if (user == null)
                {
                    _logger.LogWarning("Utilisateur {UserId} non trouvé lors de la mise à jour du solde", userId);
                    return 0;
                }

                var stripeBalance = await GetBalanceFromStripeAsync(userId);

                var result = await _userRepository.UpdateUserBalanceToExactAmountAsync(userId, stripeBalance);

                if (result)
                {
                    _logger.LogInformation("Solde local mis à jour avec succès pour l'utilisateur {UserId}: {Balance}",
                        userId, stripeBalance);
                    return stripeBalance;
                }
                else
                {
                    _logger.LogWarning("Échec de la mise à jour du solde local pour l'utilisateur {UserId}", userId);
                    return 0;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Erreur lors de la mise à jour du solde local depuis Stripe pour l'utilisateur {UserId}", userId);
                return 0;
            }
        }

        /// <summary>
        /// Retrieves the available balance in euros from Stripe for a given user.
        /// </summary>
        /// <param name="userId">The user's identifier.</param>
        /// <returns>The available balance in euros, or 0 if not available or on error.</returns>
        /// <remarks>
        /// - Returns 0 if the user is not found or does not have a Stripe account.
        /// - Logs warnings and errors for missing users, missing Stripe accounts, or Stripe API errors.
        /// </remarks>
        private async Task<decimal> GetBalanceFromStripeAsync(int userId)
        {
            try
            {
                var user = await _userRepository.GetByIdAsync(userId);
                if (user == null || string.IsNullOrEmpty(user.StripeAccountId))
                {
                    _logger.LogWarning("Impossible de récupérer le solde Stripe pour l'utilisateur {UserId}: utilisateur non trouvé ou pas de compte Stripe associé", userId);
                    return 0;
                }

                var stripeAccountService = new Stripe.BalanceService();
                var balance = await stripeAccountService.GetAsync(
                    requestOptions: new Stripe.RequestOptions { StripeAccount = user.StripeAccountId }
                );

                if (balance == null || !balance.Available.Any())
                {
                    _logger.LogInformation("Aucun solde disponible sur le compte Stripe {StripeAccountId}", user.StripeAccountId);
                    return 0;
                }

                var eurBalance = balance.Available
                    .FirstOrDefault(b => b.Currency == "eur")?.Amount ?? 0;
        
                return (decimal)eurBalance / 100;
            }
            catch (Stripe.StripeException ex)
            {
                _logger.LogError(ex, "Erreur Stripe lors de la récupération du solde pour l'utilisateur {UserId}: {Message}", userId, ex.Message);
                return 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur inattendue lors de la récupération du solde Stripe pour l'utilisateur {UserId}", userId);
                return 0;
            }
        }
    }
}