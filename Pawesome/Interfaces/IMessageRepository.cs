using Pawesome.Models.Entities;

namespace Pawesome.Interfaces;

public interface IMessageRepository : IRepository<Message>
{
    Task<IEnumerable<Message>> GetMessagesBySenderIdAsync(int senderId);
    Task<IEnumerable<Message>> GetMessagesByReceiverIdAsync(int receiverId);
    Task<IEnumerable<Message>> GetConversationAsync(int userId1, int userId2);
    Task<IEnumerable<Message>> GetLatestConversationsForUserAsync(int userId);
    Task<int> GetUnreadMessagesCountAsync(int userId);
    Task<int> GetUnreadMessagesCountFromSenderAsync(int userId, int senderId);
    Task MarkConversationAsReadAsync(int currentUserId, int otherUserId);
    Task<Message?> GetLatestMessageBetweenUsersAsync(int userId1, int userId2);
}