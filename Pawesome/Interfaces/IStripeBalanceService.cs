namespace Pawesome.Interfaces;

public interface IStripeBalanceService
{
    Task<decimal> GetUserBalanceAsync(int userId);
    Task<bool> IsOnboardingCompletedAsync(int userId);
    Task<string> CreateOnboardingLinkAsync(int userId, string returnUrl);
    Task<bool> CheckOnboardingStatusAsync(int userId);
    Task<string> CreatePayoutLinkAsync(int userId, decimal amount);
    Task<List<Models.ViewModels.Balance.PayoutHistoryViewModel>> GetUserPayoutHistoryAsync(int userId);
    public Task<decimal> UpdateLocalBalanceFromStripeAsync(int userId);
}