namespace Pawesome.Models;

public class Advert
{
    public int Id { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string Status { get; set; } = "pending";
    public decimal Amount { get; set; }
    public string? AdditionalInformation { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    
    public required ICollection<PetAdvert> PetAdverts { get; set; }
    public required ICollection<Review> Reviews { get; set; }
    public required ICollection<Payment> Payments { get; set; }
}