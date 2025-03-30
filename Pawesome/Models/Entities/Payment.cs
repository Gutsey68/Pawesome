namespace Pawesome.Models;

public class Payment
{
    public int UserId { get; set; }
    public int AdvertId { get; set; }
    public decimal Amount { get; set; }
    public string Status { get; set; } = "pending";
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    
    public User User { get; set; }
    public Advert Advert { get; set; }
}