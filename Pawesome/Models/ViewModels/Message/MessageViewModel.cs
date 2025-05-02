namespace Pawesome.Models.ViewModels.Message;

public class MessageViewModel
{
    public int Id { get; set; }
    public string Content { get; set; } = null!;
    public string Status { get; set; } = null!;
    public string FormattedDate { get; set; } = null!;
    public bool IsCurrentUserSender { get; set; }
    public int SenderId { get; set; }
    public int ReceiverId { get; set; }
    public string SenderFullName { get; set; } = null!;
    public string ReceiverFullName { get; set; } = null!;
    public string? SenderPhoto { get; set; }
}