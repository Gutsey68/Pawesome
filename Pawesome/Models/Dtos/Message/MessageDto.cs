namespace Pawesome.Models.DTOs.Message;

public class MessageDto
{
    public int Id { get; set; }
    public string Content { get; set; } = null!;
    public string Status { get; set; } = "unread";
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public int SenderId { get; set; }
    public int ReceiverId { get; set; }
    public string SenderFullName { get; set; } = null!;
    public string ReceiverFullName { get; set; } = null!;
    public string? SenderPhoto { get; set; }
}