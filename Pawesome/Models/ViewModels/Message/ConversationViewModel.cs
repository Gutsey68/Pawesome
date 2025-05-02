namespace Pawesome.Models.ViewModels.Message;

public class ConversationViewModel
{
    public int OtherUserId { get; set; }
    public string OtherUserFullName { get; set; } = null!;
    public string? OtherUserPhoto { get; set; }
    public List<MessageViewModel> Messages { get; set; } = new();
    public CreateMessageViewModel NewMessage { get; set; } = new();
}