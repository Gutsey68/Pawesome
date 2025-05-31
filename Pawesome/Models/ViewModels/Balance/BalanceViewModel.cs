namespace Pawesome.Models.ViewModels.Balance;

public class BalanceViewModel
{
    public decimal AvailableBalance { get; set; }
    public bool IsOnboardingCompleted { get; set; }
    public List<PayoutHistoryViewModel> PayoutHistory { get; set; } = new List<PayoutHistoryViewModel>();
}