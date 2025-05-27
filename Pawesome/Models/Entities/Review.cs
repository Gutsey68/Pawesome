namespace Pawesome.Models.Entities;

public class Review
{
    public int UserId { get; set; }
    public int BookingId { get; set; }

    public float Rate { get; set; }
    public string? Comment { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public required User User { get; set; }
    public required Booking Booking { get; set; }
}
