using System.ComponentModel.DataAnnotations;

namespace Pawesome.Models.Entities;

public class Message
{
    public int Id { get; set; }
    public required string Content { get; set; }
    
    [MaxLength(255)]
    public required string Status { get; set; } = "unread";
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    
    public int SenderId { get; set; }
    public int ReceiverId { get; set; }
    
    public required User Sender { get; set; }
    public required User Receiver { get; set; }
}