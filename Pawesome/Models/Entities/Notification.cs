using System.ComponentModel.DataAnnotations;
using Pawesome.Models.Enums;

namespace Pawesome.Models.Entities;

public class Notification
{
    public int Id { get; set; }
    
    [MaxLength(100)]
    public string Title { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    
    public NotificationType Type { get; set; }
    public bool IsRead { get; set; } = false;
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public int UserId { get; set; }
    public required User User { get; set; }
}