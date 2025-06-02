namespace Pawesome.Models.ViewModels.Payment
{
    public class CheckoutViewModel
    {
        public int AdvertId { get; set; }
        public int? BookingId { get; set; }
        public string ServiceType { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string StripePublishableKey { get; set; } = string.Empty;
        public bool IsPetSitter { get; set; }
    }
}
