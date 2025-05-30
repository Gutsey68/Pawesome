using System.Security.Claims;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Pawesome.Interfaces;
using Pawesome.Models.Dtos.Notification;

namespace Pawesome.Controllers;

/// <summary>
/// Controller responsible for handling notification-related operations
/// Provides endpoints for viewing, marking as read, and deleting notifications
/// </summary>
[Authorize]
[Route("[controller]")]
public class NotificationController : Controller
{
    private readonly INotificationService _notificationService;
    private readonly IMapper _mapper;

    /// <summary>
    /// Initializes a new instance of the NotificationController
    /// </summary>
    /// <param name="notificationService">Service for notification operations</param>
    /// <param name="mapper">AutoMapper instance for object mapping</param>
    public NotificationController(INotificationService notificationService, IMapper mapper)
    {
        _notificationService = notificationService;
        _mapper = mapper;
    }

    /// <summary>
    /// Retrieves and displays all notifications for the current user
    /// </summary>
    /// <returns>View containing user's notifications</returns>
    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userIdString) || !int.TryParse(userIdString, out int userId))
        {
            return Unauthorized();
        }

        var notifications = await _notificationService.GetUserNotificationsAsync(userId);
        var notificationDtos = _mapper.Map<IEnumerable<NotificationDto>>(notifications);
        return View(notificationDtos);
    }

    /// <summary>
    /// Marks a specific notification as read
    /// </summary>
    /// <param name="id">ID of the notification to mark as read</param>
    /// <returns>OK response upon successful update</returns>
    [HttpPost("MarkAsRead/{id}")]
    public async Task<IActionResult> MarkAsRead(int id)
    {
        await _notificationService.MarkAsReadAsync(id);
        return Ok();
    }

    /// <summary>
    /// Marks all notifications of the current user as read
    /// </summary>
    /// <returns>OK response upon successful batch update</returns>
    [HttpPost("MarkAllAsRead")]
    public async Task<IActionResult> MarkAllAsRead()
    {
        var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userIdString) || !int.TryParse(userIdString, out int userId))
        {
            return Unauthorized();
        }

        await _notificationService.MarkAllAsReadAsync(userId);
        return Ok();
    }

    /// <summary>
    /// Deletes a specific notification
    /// </summary>
    /// <param name="id">ID of the notification to delete</param>
    /// <returns>OK response upon successful deletion</returns>
    [HttpPost("Delete/{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _notificationService.DeleteNotificationAsync(id);
        return Ok();
    }

    /// <summary>
    /// Gets the count of unread notifications for the current user
    /// Used by the notification badge in the UI
    /// </summary>
    /// <returns>JSON object containing the unread notification count</returns>
    [HttpGet("GetUnreadCount")]
    public async Task<IActionResult> GetUnreadCount()
    {
        var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userIdString) || !int.TryParse(userIdString, out int userId))
        {
            return Unauthorized();
        }

        var count = await _notificationService.GetUnreadCountAsync(userId);
        return Ok(new { count });
    }
    
    /// <summary>
    /// Gets notifications for the current user to display in the navbar popup
    /// </summary>
    /// <returns>JSON array of notification objects</returns>
    [HttpGet("GetNotifications")]
    public async Task<IActionResult> GetNotifications()
    {
        var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userIdString) || !int.TryParse(userIdString, out int userId))
        {
            return Unauthorized();
        }

        var notifications = await _notificationService.GetUserNotificationsAsync(userId);
        var notificationDtos = _mapper.Map<IEnumerable<NotificationDto>>(notifications);
    
        return Ok(notificationDtos.OrderByDescending(n => n.CreatedAt).Take(10));
    }
}
