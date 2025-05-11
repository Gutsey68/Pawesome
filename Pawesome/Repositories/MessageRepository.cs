using Microsoft.EntityFrameworkCore;
using Pawesome.Data;
using Pawesome.Interfaces;
using Pawesome.Models.Entities;

namespace Pawesome.Repositories;

/// <summary>
/// Repository implementation for managing message data operations
/// </summary>
/// <remarks>
/// This repository provides methods for retrieving, manipulating,
/// and managing message data within the application database.
/// </remarks>
public class MessageRepository : Repository<Message>, IMessageRepository
{
    /// <summary>
    /// Initializes a new instance of the MessageRepository class
    /// </summary>
    /// <param name="context">Database context for message operations</param>
    public MessageRepository(AppDbContext context) : base(context)
    {
    }

    /// <summary>
    /// Retrieves all messages sent by a specific user
    /// </summary>
    /// <param name="senderId">ID of the user who sent the messages</param>
    /// <returns>Collection of messages sent by the specified user</returns>
    public async Task<IEnumerable<Message>> GetMessagesBySenderIdAsync(int senderId)
    {
        return await _context.Messages
            .Include(m => m.Sender)
            .Include(m => m.Receiver)
            .Where(m => m.SenderId == senderId)
            .OrderByDescending(m => m.CreatedAt)
            .ToListAsync();
    }

    /// <summary>
    /// Retrieves all messages received by a specific user
    /// </summary>
    /// <param name="receiverId">ID of the user who received the messages</param>
    /// <returns>Collection of messages received by the specified user</returns>
    public async Task<IEnumerable<Message>> GetMessagesByReceiverIdAsync(int receiverId)
    {
        return await _context.Messages
            .Include(m => m.Sender)
            .Include(m => m.Receiver)
            .Where(m => m.ReceiverId == receiverId)
            .OrderByDescending(m => m.CreatedAt)
            .ToListAsync();
    }

    /// <summary>
    /// Retrieves the complete conversation history between two users
    /// </summary>
    /// <param name="userId1">ID of the first user in the conversation</param>
    /// <param name="userId2">ID of the second user in the conversation</param>
    /// <returns>Collection of messages exchanged between the two users</returns>
    public async Task<IEnumerable<Message>> GetConversationAsync(int userId1, int userId2)
    {
        return await _context.Messages
            .Include(m => m.Sender)
            .Include(m => m.Receiver)
            .Where(m => (m.SenderId == userId1 && m.ReceiverId == userId2) ||
                        (m.SenderId == userId2 && m.ReceiverId == userId1))
            .OrderBy(m => m.CreatedAt)
            .ToListAsync();
    }

    /// <summary>
    /// Retrieves the latest messages for each conversation involving the specified user
    /// </summary>
    /// <param name="userId">ID of the user whose conversations should be retrieved</param>
    /// <returns>Collection of the latest message from each conversation</returns>
    public async Task<IEnumerable<Message>> GetLatestConversationsForUserAsync(int userId)
    {
        var conversationUsers = await _context.Messages
            .Where(m => m.SenderId == userId || m.ReceiverId == userId)
            .Select(m => m.SenderId == userId ? m.ReceiverId : m.SenderId)
            .Distinct()
            .ToListAsync();

        var latestMessages = new List<Message>();
        foreach(var otherUserId in conversationUsers)
        {
            var latestMessage = await _context.Messages
                .Include(m => m.Sender)
                .Include(m => m.Receiver)
                .Where(m => (m.SenderId == userId && m.ReceiverId == otherUserId) ||
                            (m.SenderId == otherUserId && m.ReceiverId == userId))
                .OrderByDescending(m => m.CreatedAt)
                .FirstOrDefaultAsync();

            if(latestMessage != null)
            {
                latestMessages.Add(latestMessage);
            }
        }

        return latestMessages.OrderByDescending(m => m.CreatedAt);
    }

    /// <summary>
    /// Counts the number of unread messages for a specific user
    /// </summary>
    /// <param name="userId">ID of the user whose unread messages should be counted</param>
    /// <returns>The number of unread messages</returns>
    public async Task<int> GetUnreadMessagesCountAsync(int userId)
    {
        return await _context.Messages
            .CountAsync(m => m.ReceiverId == userId && m.Status == "unread");
    }

    /// <summary>
    /// Counts the number of unread messages sent by a specific user to another user
    /// </summary>
    /// <param name="userId">ID of the message recipient</param>
    /// <param name="senderId">ID of the message sender</param>
    /// <returns>The number of unread messages from the sender to the recipient</returns>
    public async Task<int> GetUnreadMessagesCountFromSenderAsync(int userId, int senderId)
    {
        return await _context.Messages
            .CountAsync(m => m.ReceiverId == userId && m.SenderId == senderId && m.Status == "unread");
    }

    /// <summary>
    /// Marks all messages in a conversation as read
    /// </summary>
    /// <param name="currentUserId">ID of the current user (recipient)</param>
    /// <param name="otherUserId">ID of the other user (sender)</param>
    /// <returns>A task representing the asynchronous operation</returns>
    public async Task MarkConversationAsReadAsync(int currentUserId, int otherUserId)
    {
        var unreadMessages = await _context.Messages
            .Where(m => m.ReceiverId == currentUserId && m.SenderId == otherUserId && m.Status == "unread")
            .ToListAsync();

        foreach (var message in unreadMessages)
        {
            message.Status = "read";
            message.UpdatedAt = DateTime.UtcNow;
        }
    }

    /// <summary>
    /// Retrieves the latest message exchanged between two users
    /// </summary>
    /// <param name="userId1">ID of the first user</param>
    /// <param name="userId2">ID of the second user</param>
    /// <returns>The most recent message between the two users, or null if no messages exist</returns>
    public async Task<Message?> GetLatestMessageBetweenUsersAsync(int userId1, int userId2)
    {
        return await _context.Messages
            .Include(m => m.Sender)
            .Include(m => m.Receiver)
            .Where(m => (m.SenderId == userId1 && m.ReceiverId == userId2) ||
                        (m.SenderId == userId2 && m.ReceiverId == userId1))
            .OrderByDescending(m => m.CreatedAt)
            .FirstOrDefaultAsync();
    }
}