using Pawesome.Models.Dtos.Payment;
using Pawesome.Models.enums;

namespace Pawesome.Models.Dtos.Booking;

public class BookingDto
{
    public int Id { get; set; }
    public BookingStatus Status { get; set; }
    public decimal Amount { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public bool IsValidated { get; set; }
    public DateTime? ValidatedAt { get; set; }
    public bool IsDisputed { get; set; }
    public string? DisputeReason { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
        
    public int AdvertId { get; set; }
    public string AdvertTitle { get; set; } = string.Empty;
        
    public int BookerUserId { get; set; }
    public string BookerUserName { get; set; } = string.Empty;
    public string BookerUserEmail { get; set; } = string.Empty;
        
    public int PetSitterUserId { get; set; }
    public string PetSitterUserName { get; set; } = string.Empty;
        
    public List<PaymentDto> Payments { get; set; } = new List<PaymentDto>();
}