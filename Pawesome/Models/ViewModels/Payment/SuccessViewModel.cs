using System;

namespace Pawesome.Models.ViewModels.Payment
{

    public class SuccessViewModel
    {
        public int BookingId { get; set; }
        public decimal Amount { get; set; }
        public DateTime PaymentDate { get; set; } = DateTime.UtcNow;
        public bool IsAutoAccepted { get; set; } = false;
    }
}