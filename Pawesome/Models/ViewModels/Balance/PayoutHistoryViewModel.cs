namespace Pawesome.Models.ViewModels.Balance;

public class PayoutHistoryViewModel
{
    public required string Id { get; set; }
    public decimal Amount { get; set; }
    public required string Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? ArrivalDate { get; set; }
}