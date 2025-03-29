namespace Pawesome.Models;

public class Message
{
    public int Id { get; set; }
    public string Content { get; set; }
    public string Status { get; set; } = "unread";
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    
    public int SenderId { get; set; }
    public int ReceiverId { get; set; }
    
    public User Sender { get; set; }
    public User Receiver { get; set; }
}