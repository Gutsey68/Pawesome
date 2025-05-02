namespace Pawesome.Models.DTOs.Message;

public class CreateMessageDto
{
    public string Content { get; set; } = null!;
    public int ReceiverId { get; set; }
}