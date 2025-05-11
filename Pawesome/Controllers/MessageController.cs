using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Pawesome.Hubs;
using Pawesome.Interfaces;
using Pawesome.Models.DTOs.Message;
using Pawesome.Models.ViewModels.Message;
using System.Security.Claims;
using AutoMapper;

namespace Pawesome.Controllers;

/// <summary>
/// Controller for managing messaging between users
/// </summary>
public class MessageController : Controller
{
    private readonly IMessageService _messageService;
    private readonly IUserService _userService;
    private readonly IMapper _mapper;
    private readonly IHubContext<MessageHub> _hubContext;

    /// <summary>
    /// Constructor for MessageController
    /// </summary>
    public MessageController(
        IMessageService messageService,
        IUserService userService,
        IMapper mapper,
        IHubContext<MessageHub> hubContext)
    {
        _messageService = messageService;
        _userService = userService;
        _mapper = mapper;
        _hubContext = hubContext;
    }

    /// <summary>
    /// Displays the list of all conversations for the current user
    /// </summary>
    /// <returns>View with the list of conversations</returns>
    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
    
        if (string.IsNullOrEmpty(userIdStr) || !int.TryParse(userIdStr, out var userId))
        {
            return Unauthorized();
        }

        var conversationsDto = await _messageService.GetLatestConversationsForUserAsync(userId);

        var viewModel = new ConversationsListViewModel
        {
            Conversations = []
        };

        foreach (var conversation in conversationsDto)
        {
            var otherUserId = conversation.SenderId == userId ? conversation.ReceiverId : conversation.SenderId;
            var otherUserFullName = conversation.SenderId == userId ? conversation.ReceiverFullName : conversation.SenderFullName;
            var otherUserPhoto = conversation.SenderId == userId ? conversation.ReceiverPhoto : conversation.SenderPhoto;

            var unreadCount = await _messageService.GetUnreadMessagesCountFromSenderAsync(userId, otherUserId);

            viewModel.Conversations.Add(new MessageSummaryViewModel
            {
                UserId = otherUserId,
                UserFullName = otherUserFullName,
                UserPhoto = otherUserPhoto,
                LastMessageContent = conversation.Content,
                FormattedDate = conversation.CreatedAt.ToString("dd/MM/yyyy HH:mm"),
                UnreadCount = unreadCount
            });
        }

        ViewBag.ConversationsViewModel = viewModel;
        return View(viewModel);
    }

    /// <summary>
    /// Displays a conversation with another user
    /// </summary>
    /// <param name="otherUserId">The ID of the other user in the conversation</param>
    /// <returns>View with the conversation history and message input form</returns>
    [HttpGet("{otherUserId:int}")]
    public async Task<IActionResult> Conversation(int otherUserId)
    {
        var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!int.TryParse(userIdString, out var currentUserId))
        {
            return RedirectToAction("Login", "Auth");
        }

        var otherUser = await _userService.GetUserProfileAsync(otherUserId);
        if (otherUser == null)
        {
            return NotFound();
        }

        await _messageService.MarkConversationAsReadAsync(currentUserId, otherUserId);

        var conversationsDto = await _messageService.GetLatestConversationsForUserAsync(currentUserId);
        var conversationsViewModel = new ConversationsListViewModel
        {
            Conversations = []
        };

        foreach (var conversation in conversationsDto)
        {
            var convOtherUserId = conversation.SenderId == currentUserId ? conversation.ReceiverId : conversation.SenderId;
            var convOtherUserFullName = conversation.SenderId == currentUserId ? conversation.ReceiverFullName : conversation.SenderFullName;
            var convOtherUserPhoto = conversation.SenderId == currentUserId ? conversation.ReceiverPhoto : conversation.SenderPhoto;

            var unreadCount = await _messageService.GetUnreadMessagesCountFromSenderAsync(currentUserId, convOtherUserId);

            conversationsViewModel.Conversations.Add(new MessageSummaryViewModel
            {
                UserId = convOtherUserId,
                UserFullName = convOtherUserFullName,
                UserPhoto = convOtherUserPhoto,
                LastMessageContent = conversation.Content,
                FormattedDate = conversation.CreatedAt.ToString("dd/MM/yyyy HH:mm"),
                UnreadCount = unreadCount
            });
        }

        ViewBag.ConversationsViewModel = conversationsViewModel;

        var messagesDto = await _messageService.GetConversationAsync(currentUserId, otherUserId);
        var messages = _mapper.Map<List<MessageViewModel>>(messagesDto);

        foreach (var message in messages)
        {
            message.IsCurrentUserSender = message.SenderId == currentUserId;
        }

        var viewModel = new ConversationViewModel
        {
            OtherUserId = otherUserId,
            OtherUserFullName = $"{otherUser.FirstName} {otherUser.LastName}",
            OtherUserPhoto = otherUser.Photo,
            Messages = messages,
            NewMessage = new CreateMessageViewModel { ReceiverId = otherUserId }
        };

        return View(viewModel);
    }

    /// <summary>
    /// Entry point for creating a conversation from a user profile
    /// </summary>
    /// <param name="otherUserId">The ID of the other user to start a conversation with</param>
    /// <returns>Redirects to the conversation view</returns>
    [HttpGet("conversation")]
    public async Task<IActionResult> StartConversation(int otherUserId)
    {
        var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
        
        if (string.IsNullOrEmpty(userIdStr) || !int.TryParse(userIdStr, out int userId))
        {
            return Unauthorized();
        }

        var otherUser = await _userService.GetUserProfileAsync(otherUserId);
        if (otherUser == null)
        {
            return NotFound();
        }

        return RedirectToAction("Conversation", new { otherUserId });
    }

    /// <summary>
    /// Processes the form submission for sending a new message
    /// </summary>
    /// <param name="viewModel">The message creation view model</param>
    /// <returns>Redirects back to the conversation</returns>
    [HttpPost("send")]
    public async Task<IActionResult> SendMessage([FromForm] CreateMessageViewModel viewModel)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
        
        if (string.IsNullOrEmpty(userIdStr) || !int.TryParse(userIdStr, out var userId))
        {
            return Unauthorized();
        }

        var messageDto = new CreateMessageDto
        {
            Content = viewModel.Content,
            ReceiverId = viewModel.ReceiverId
        };

        var createdMessage = await _messageService.CreateMessageAsync(messageDto, userId);

        await _hubContext.Clients.User(viewModel.ReceiverId.ToString())
            .SendAsync("ReceiveMessage", createdMessage);

        await _hubContext.Clients.User(userId.ToString())
            .SendAsync("MessageSent", createdMessage);

        return RedirectToAction("Conversation", new { otherUserId = viewModel.ReceiverId });
    }

    /// <summary>
    /// API endpoint for sending a message via AJAX
    /// </summary>
    /// <param name="viewModel">The message creation view model</param>
    /// <returns>JSON result with the created message</returns>
    [HttpPost("api/send")]
    public async Task<IActionResult> ApiSendMessage([FromBody] CreateMessageViewModel viewModel)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
        
        if (string.IsNullOrEmpty(userIdStr) || !int.TryParse(userIdStr, out var userId))
        {
            return Unauthorized();
        }

        var messageDto = new CreateMessageDto
        {
            Content = viewModel.Content,
            ReceiverId = viewModel.ReceiverId
        };

        var createdMessage = await _messageService.CreateMessageAsync(messageDto, userId);

        await _hubContext.Clients.User(viewModel.ReceiverId.ToString())
            .SendAsync("ReceiveMessage", createdMessage);

        await _hubContext.Clients.User(userId.ToString())
            .SendAsync("MessageSent", createdMessage);

        return Ok(createdMessage);
    }

    /// <summary>
    /// API endpoint for marking a conversation as read
    /// </summary>
    /// <param name="otherUserId">The ID of the other user in the conversation</param>
    /// <returns>OK result if successful</returns>
    [HttpPost("api/mark-read/{otherUserId:int}")]
    public async Task<IActionResult> MarkAsRead(int otherUserId)
    {
        var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
        
        if (string.IsNullOrEmpty(userIdStr) || !int.TryParse(userIdStr, out var userId))
        {
            return Unauthorized();
        }

        await _messageService.MarkConversationAsReadAsync(userId, otherUserId);

        await _hubContext.Clients.User(otherUserId.ToString())
            .SendAsync("MessagesRead", userId);

        return Ok();
    }


    /// <summary>
    /// API endpoint for getting the count of unread messages
    /// </summary>
    /// <returns>JSON result with the count of unread messages</returns>
    [HttpGet("api/unread-count")]
    public async Task<IActionResult> GetUnreadCount()
    {
        var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
        
        if (string.IsNullOrEmpty(userIdStr) || !int.TryParse(userIdStr, out int userId))
        {
            return Unauthorized();
        }

        var count = await _messageService.GetUnreadMessagesCountAsync(userId);
        
        return Ok(new { count });
    }
}
