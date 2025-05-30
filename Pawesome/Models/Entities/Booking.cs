using Pawesome.Models.enums;

namespace Pawesome.Models.Entities;

public class Booking
{
    public int Id { get; set; }
    public BookingStatus Status { get; set; } = BookingStatus.PendingConfirmation;
    public decimal Amount { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public bool IsValidated { get; set; } = false;
    public DateTime? ValidatedAt { get; set; }
    public bool IsDisputed { get; set; } = false;
    public string? DisputeReason { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    
    public int AdvertId { get; set; }
    public int BookerUserId { get; set; }
    
    public required Advert Advert { get; set; }
    public required User BookerUser { get; set; }
    public required ICollection<Payment> Payments { get; set; }
}