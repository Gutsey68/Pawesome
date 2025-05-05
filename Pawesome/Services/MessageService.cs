using AutoMapper;
using Pawesome.Interfaces;
using Pawesome.Models.DTOs.Message;
using Pawesome.Models.Entities;

namespace Pawesome.Services;

/// <summary>
/// Service for handling message-related operations
/// </summary>
/// <remarks>
/// This service provides business logic for managing messages between users,
/// including retrieving conversations, creating messages, and tracking read status.
/// </remarks>
public class MessageService : IMessageService
{
    private readonly IMessageRepository _messageRepository;
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;

    /// <summary>
    /// Initializes a new instance of the MessageService class
    /// </summary>
    /// <param name="messageRepository">Repository for message data operations</param>
    /// <param name="userRepository">Repository for user data operations</param>
    /// <param name="mapper">AutoMapper instance for object mapping</param>
    public MessageService(IMessageRepository messageRepository, IUserRepository userRepository, IMapper mapper)
    {
        _messageRepository = messageRepository;
        _userRepository = userRepository;
        _mapper = mapper;
    }

    /// <summary>
    /// Retrieves all messages sent by a specific user
    /// </summary>
    /// <param name="senderId">ID of the user who sent the messages</param>
    /// <returns>Collection of message DTOs sent by the specified user</returns>
    public async Task<IEnumerable<MessageDto>> GetMessagesBySenderIdAsync(int senderId)
    {
        var messages = await _messageRepository.GetMessagesBySenderIdAsync(senderId);
        return _mapper.Map<IEnumerable<MessageDto>>(messages);
    }

    /// <summary>
    /// Retrieves all messages received by a specific user
    /// </summary>
    /// <param name="receiverId">ID of the user who received the messages</param>
    /// <returns>Collection of message DTOs received by the specified user</returns>
    public async Task<IEnumerable<MessageDto>> GetMessagesByReceiverIdAsync(int receiverId)
    {
        var messages = await _messageRepository.GetMessagesByReceiverIdAsync(receiverId);
        return _mapper.Map<IEnumerable<MessageDto>>(messages);
    }

    /// <summary>
    /// Retrieves the complete conversation history between two users
    /// </summary>
    /// <param name="userId1">ID of the first user in the conversation</param>
    /// <param name="userId2">ID of the second user in the conversation</param>
    /// <returns>Collection of message DTOs exchanged between the two users</returns>
    public async Task<IEnumerable<MessageDto>> GetConversationAsync(int userId1, int userId2)
    {
        var messages = await _messageRepository.GetConversationAsync(userId1, userId2);
        return _mapper.Map<IEnumerable<MessageDto>>(messages) ?? new List<MessageDto>();
    }

    /// <summary>
    /// Retrieves the latest messages for each conversation involving the specified user
    /// </summary>
    /// <param name="userId">ID of the user whose conversations should be retrieved</param>
    /// <returns>Collection of the latest message DTOs from each conversation</returns>
    public async Task<IEnumerable<MessageDto>> GetLatestConversationsForUserAsync(int userId)
    {
        var conversations = await _messageRepository.GetLatestConversationsForUserAsync(userId);
        return _mapper.Map<IEnumerable<MessageDto>>(conversations);
    }

    /// <summary>
    /// Retrieves a message by its ID
    /// </summary>
    /// <param name="id">ID of the message to retrieve</param>
    /// <returns>The message DTO if found, null otherwise</returns>
    public async Task<MessageDto?> GetMessageByIdAsync(int id)
    {
        var message = await _messageRepository.GetByIdAsync(id);
        return message != null ? _mapper.Map<MessageDto>(message) : null;
    }

    /// <summary>
    /// Creates a new message between users
    /// </summary>
    /// <param name="messageDto">DTO containing message creation data</param>
    /// <param name="currentUserId">ID of the current user sending the message</param>
    /// <returns>The created message as a DTO</returns>
    /// <exception cref="ArgumentException">Thrown when sender or receiver does not exist</exception>
    public async Task<MessageDto> CreateMessageAsync(CreateMessageDto messageDto, int currentUserId)
    {
        var sender = await _userRepository.GetByIdAsync(currentUserId);
        var receiver = await _userRepository.GetByIdAsync(messageDto.ReceiverId);

        if (sender == null || receiver == null)
            throw new ArgumentException("Sender or receiver does not exist");

        var message = new Message
        {
            Content = messageDto.Content,
            Status = "unread",
            SenderId = currentUserId,
            ReceiverId = messageDto.ReceiverId,
            Sender = sender,
            Receiver = receiver
        };

        await _messageRepository.AddAsync(message);
        await _messageRepository.SaveChangesAsync();

        return _mapper.Map<MessageDto>(message);
    }

    /// <summary>
    /// Marks all messages in a conversation as read
    /// </summary>
    /// <param name="currentUserId">ID of the current user (recipient)</param>
    /// <param name="otherUserId">ID of the other user (sender)</param>
    /// <returns>A task representing the asynchronous operation</returns>
    public async Task MarkConversationAsReadAsync(int currentUserId, int otherUserId)
    {
        await _messageRepository.MarkConversationAsReadAsync(currentUserId, otherUserId);
        await _messageRepository.SaveChangesAsync();
    }

    /// <summary>
    /// Counts the number of unread messages for a specific user
    /// </summary>
    /// <param name="userId">ID of the user whose unread messages should be counted</param>
    /// <returns>The number of unread messages</returns>
    public async Task<int> GetUnreadMessagesCountAsync(int userId)
    {
        return await _messageRepository.GetUnreadMessagesCountAsync(userId);
    }

    /// <summary>
    /// Counts the number of unread messages sent by a specific user to another user
    /// </summary>
    /// <param name="userId">ID of the message recipient</param>
    /// <param name="senderId">ID of the message sender</param>
    /// <returns>The number of unread messages from the sender to the recipient</returns>
    public async Task<int> GetUnreadMessagesCountFromSenderAsync(int userId, int senderId)
    {
        return await _messageRepository.GetUnreadMessagesCountFromSenderAsync(userId, senderId);
    }

    /// <summary>
    /// Deletes a message by its ID
    /// </summary>
    /// <param name="messageId">ID of the message to delete</param>
    /// <returns>A task representing the asynchronous operation</returns>
    public async Task DeleteMessageAsync(int messageId)
    {
        var message = await _messageRepository.GetByIdAsync(messageId);
        if (message != null)
        {
            await _messageRepository.DeleteAsync(message);
            await _messageRepository.SaveChangesAsync();
        }
    }
}