namespace Pawesome.Models.ViewModels.Message;

public class MessageSummaryViewModel
{
    public int UserId { get; set; }
    public string UserFullName { get; set; } = null!;
    public string? UserPhoto { get; set; }
    public string LastMessageContent { get; set; } = null!;
    public string FormattedDate { get; set; } = null!;
    public int UnreadCount { get; set; }
}