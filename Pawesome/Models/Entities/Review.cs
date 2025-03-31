namespace Pawesome.Models;

public class Review
{
    public int UserId { get; set; }
    public int AdvertId { get; set; }
    public float Rate { get; set; }
    public string? Comment { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    
    public required User User { get; set; }
    public required Advert Advert { get; set; }
}