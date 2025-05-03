namespace Pawesome.Models.ViewModels.Payment
{
    public class PaymentHistoryViewModel
    {
        public int Id { get; set; }
        public int AdvertId { get; set; }
        public decimal Amount { get; set; }
        public string Status { get; set; } = string.Empty;
        public string? SessionId { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsPetSitter { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}