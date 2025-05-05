using System.ComponentModel.DataAnnotations;

namespace Pawesome.Models.Entities;

public class Payment
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int AdvertId { get; set; }
    public decimal Amount { get; set; }
    
    [MaxLength(255)]
    public string Status { get; set; } = "pending";
    
    [MaxLength(255)]
    public string? SessionId { get; set; }
    
    [MaxLength(255)]
    public string? PaymentIntentId { get; set; }
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    
    public required User User { get; set; }
    public required Advert Advert { get; set; }
}