using Microsoft.EntityFrameworkCore;
using Pawesome.Data;
using Pawesome.Interfaces;
using Pawesome.Models.Entities;

namespace Pawesome.Repositories;

/// <summary>
/// Repository for managing notification data access operations
/// Extends the base Repository with notification-specific operations
/// </summary>
public class NotificationRepository : Repository<Notification>, INotificationRepository
{
    /// <summary>
    /// Initializes a new instance of the NotificationRepository
    /// </summary>
    /// <param name="context">Database context for data operations</param>
    public NotificationRepository(AppDbContext context) : base(context)
    {
    }
    
    /// <summary>
    /// Retrieves notifications for a specific user with pagination
    /// </summary>
    /// <param name="userId">ID of the user whose notifications to retrieve</param>
    /// <param name="skip">Number of items to skip for pagination</param>
    /// <param name="take">Number of items to take per page</param>
    /// <returns>Collection of notifications for the specified user</returns>
    public async Task<IEnumerable<Notification>> GetNotificationsForUserAsync(int userId, int skip = 0, int take = 20)
    {
        return await _context.Notifications
            .Where(n => n.UserId == userId)
            .OrderByDescending(n => n.CreatedAt)
            .Skip(skip)
            .Take(take)
            .ToListAsync();
    }
    
    /// <summary>
    /// Gets the count of unread notifications for a specific user
    /// </summary>
    /// <param name="userId">ID of the user</param>
    /// <returns>Count of unread notifications</returns>
    public async Task<int> GetUnreadCountAsync(int userId)
    {
        return await _context.Notifications
            .CountAsync(n => n.UserId == userId && !n.IsRead);
    }
    
    /// <summary>
    /// Creates a new notification in the database
    /// </summary>
    /// <param name="notification">Notification entity to create</param>
    /// <returns>The created notification with updated identity</returns>
    public async Task<Notification> CreateAsync(Notification notification)
    {
        await AddAsync(notification);
        await SaveChangesAsync();
        return notification;
    }
    
    /// <summary>
    /// Marks a specific notification as read
    /// </summary>
    /// <param name="notificationId">ID of the notification to mark as read</param>
    /// <returns>Task representing the asynchronous operation</returns>
    public async Task MarkAsReadAsync(int notificationId)
    {
        var notification = await _context.Notifications.FindAsync(notificationId);
        if (notification != null)
        {
            notification.IsRead = true;
            notification.UpdatedAt = DateTime.UtcNow;
            await SaveChangesAsync();
        }
    }
    
    /// <summary>
    /// Marks all notifications of a specific user as read
    /// </summary>
    /// <param name="userId">ID of the user whose notifications to mark as read</param>
    /// <returns>Task representing the asynchronous operation</returns>
    public async Task MarkAllAsReadAsync(int userId)
    {
        var notifications = await _context.Notifications
            .Where(n => n.UserId == userId && !n.IsRead)
            .ToListAsync();

        foreach (var notification in notifications)
        {
            notification.IsRead = true;
            notification.UpdatedAt = DateTime.UtcNow;
        }

        await SaveChangesAsync();
    }
    
    /// <summary>
    /// Deletes a specific notification from the database
    /// </summary>
    /// <param name="notificationId">ID of the notification to delete</param>
    /// <returns>Task representing the asynchronous operation</returns>
    public async Task DeleteAsync(int notificationId)
    {
        var notification = await _context.Notifications.FindAsync(notificationId);
        if (notification != null)
        {
            await DeleteAsync(notification);
            await SaveChangesAsync();
        }
    }
}
