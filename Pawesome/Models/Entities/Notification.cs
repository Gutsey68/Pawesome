namespace Pawesome.Models.Entities;

public class Notification
{
    public int Id { get; set; }
    public string? Comment { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    
    public int UserId { get; set; }
    
    public required User User { get; set; }
}