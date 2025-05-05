using Pawesome.Models.DTOs;
using Pawesome.Models.DTOs.Message;

namespace Pawesome.Interfaces;

public interface IMessageService
{
    Task<IEnumerable<MessageDto>> GetMessagesBySenderIdAsync(int senderId);
    Task<IEnumerable<MessageDto>> GetMessagesByReceiverIdAsync(int receiverId);
    Task<IEnumerable<MessageDto>> GetConversationAsync(int userId1, int userId2);
    Task<IEnumerable<MessageDto>> GetLatestConversationsForUserAsync(int userId);
    Task<MessageDto?> GetMessageByIdAsync(int id);
    Task<MessageDto> CreateMessageAsync(CreateMessageDto messageDto, int currentUserId);
    Task MarkConversationAsReadAsync(int currentUserId, int otherUserId);
    Task<int> GetUnreadMessagesCountAsync(int userId);
    Task<int> GetUnreadMessagesCountFromSenderAsync(int userId, int senderId);
    Task DeleteMessageAsync(int messageId);
}