using System.ComponentModel.DataAnnotations;

namespace Pawesome.Models.Entities;

public class Advert
{
    public int Id { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    
    [MaxLength(255)]
    public string Status { get; set; } = "pending";
    
    public decimal Amount { get; set; }
    
    [MaxLength(255)]
    public string? AdditionalInformation { get; set; }
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    
    public int UserId { get; set; }
    public User User { get; set; } = null!;
    
    public int AddressId { get; set; }
    public required Address Address { get; set; }
    
    public required ICollection<PetAdvert> PetAdverts { get; set; }
    public required ICollection<Review> Reviews { get; set; }
    public required ICollection<Payment> Payments { get; set; }
}