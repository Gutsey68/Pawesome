namespace Pawesome.Models;

public class Report
{
    public int Id { get; set; }
    public string? Comment { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    
    public int UserId { get; set; }
    
    public required User User { get; set; }
}