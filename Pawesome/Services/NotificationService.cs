using AutoMapper;
using Microsoft.AspNetCore.SignalR;
using Pawesome.Hubs;
using Pawesome.Interfaces;
using Pawesome.Models.Dtos.Notification;
using Pawesome.Models.Entities;

namespace Pawesome.Services;

/// <summary>
/// Service for managing notification operations
/// Handles the business logic between controllers and repositories 
/// and provides real-time notification capabilities
/// </summary>
public class NotificationService : INotificationService
{
    private readonly INotificationRepository _notificationRepository;
    private readonly IHubContext<NotificationHub> _notificationHubContext;
    private readonly IMapper _mapper;

    /// <summary>
    /// Initializes a new instance of the NotificationService
    /// </summary>
    /// <param name="notificationRepository">Repository for notification data access</param>
    /// <param name="notificationHubContext">SignalR hub context for real-time communications</param>
    /// <param name="mapper">AutoMapper instance for object mapping</param>
    public NotificationService(
        INotificationRepository notificationRepository,
        IHubContext<NotificationHub> notificationHubContext,
        IMapper mapper)
    {
        _notificationRepository = notificationRepository;
        _notificationHubContext = notificationHubContext;
        _mapper = mapper;
    }

    /// <summary>
    /// Retrieves notifications for a specific user with pagination
    /// </summary>
    /// <param name="userId">ID of the user whose notifications to retrieve</param>
    /// <param name="skip">Number of items to skip for pagination</param>
    /// <param name="take">Number of items to take per page</param>
    /// <returns>Collection of notifications for the specified user</returns>
    public async Task<IEnumerable<Notification>> GetUserNotificationsAsync(int userId, int skip = 0, int take = 20)
    {
        return await _notificationRepository.GetNotificationsForUserAsync(userId, skip, take);
    }

    /// <summary>
    /// Gets the count of unread notifications for a specific user
    /// </summary>
    /// <param name="userId">ID of the user</param>
    /// <returns>Count of unread notifications</returns>
    public async Task<int> GetUnreadCountAsync(int userId)
    {
        return await _notificationRepository.GetUnreadCountAsync(userId);
    }

    /// <summary>
    /// Creates a new notification and sends it in real-time to the user
    /// </summary>
    /// <param name="notification">Notification entity to create</param>
    /// <returns>The created notification with updated identity</returns>
    public async Task<Notification> CreateNotificationAsync(Notification notification)
    {
        var createdNotification = await _notificationRepository.CreateAsync(notification);
        await SendRealTimeNotificationAsync(notification.UserId, createdNotification);
        return createdNotification;
    }

    /// <summary>
    /// Marks a specific notification as read
    /// </summary>
    /// <param name="notificationId">ID of the notification to mark as read</param>
    /// <returns>Task representing the asynchronous operation</returns>
    public async Task MarkAsReadAsync(int notificationId)
    {
        await _notificationRepository.MarkAsReadAsync(notificationId);
    }

    /// <summary>
    /// Marks all notifications of a specific user as read
    /// </summary>
    /// <param name="userId">ID of the user whose notifications to mark as read</param>
    /// <returns>Task representing the asynchronous operation</returns>
    public async Task MarkAllAsReadAsync(int userId)
    {
        await _notificationRepository.MarkAllAsReadAsync(userId);
    }

    /// <summary>
    /// Deletes a specific notification
    /// </summary>
    /// <param name="notificationId">ID of the notification to delete</param>
    /// <returns>Task representing the asynchronous operation</returns>
    public async Task DeleteNotificationAsync(int notificationId)
    {
        await _notificationRepository.DeleteAsync(notificationId);
    }

    /// <summary>
    /// Sends a real-time notification to a specific user via SignalR
    /// </summary>
    /// <param name="userId">ID of the user to receive the notification</param>
    /// <param name="notification">Notification to send</param>
    /// <returns>Task representing the asynchronous operation</returns>
    public async Task SendRealTimeNotificationAsync(int userId, Notification notification)
    {
        var notificationDto = _mapper.Map<NotificationDto>(notification);
        await _notificationHubContext.Clients.Group($"User_{userId}")
            .SendAsync("ReceiveNotification", notificationDto);
    }
}
