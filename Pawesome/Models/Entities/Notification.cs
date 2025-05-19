using System.ComponentModel.DataAnnotations;

namespace Pawesome.Models.Entities;

public enum NotificationType
{
    Message,
    AdvertUpdate,
    BookingRequest,
    BookingStatusChange,
    SystemAlert
}

public class Notification
{
    public int Id { get; set; }
    
    [MaxLength(100)]
    public string Title { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    
    public NotificationType Type { get; set; }
    public bool IsRead { get; set; } = false;
    
    [MaxLength(250)]
    public string? LinkUrl { get; set; }
    
    [MaxLength(250)]
    public string? ImageUrl { get; set; }
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public int UserId { get; set; }
    public required User User { get; set; }
}