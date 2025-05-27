using System.ComponentModel.DataAnnotations;
using Pawesome.Models.Enums;

namespace Pawesome.Models.Entities;

public class Payment
{
    public int Id { get; set; }

    public decimal Amount { get; set; }

    public PaymentStatus Status { get; set; } = PaymentStatus.Pending;

    [MaxLength(255)]
    public string? SessionId { get; set; }

    [MaxLength(255)]
    public string? PaymentIntentId { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    
    public int BookingId { get; set; }
    public required Booking Booking { get; set; }
}