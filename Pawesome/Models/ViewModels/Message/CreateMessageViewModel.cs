namespace Pawesome.Models.ViewModels.Message;

public class CreateMessageViewModel
{
    public string Content { get; set; } = null!;
    public int ReceiverId { get; set; }
}