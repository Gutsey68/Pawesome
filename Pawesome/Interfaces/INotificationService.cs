using Pawesome.Models.Entities;

namespace Pawesome.Interfaces;

public interface INotificationService
{
    Task<IEnumerable<Notification>> GetUserNotificationsAsync(int userId, int skip = 0, int take = 20);
    Task<int> GetUnreadCountAsync(int userId);
    Task<Notification> CreateNotificationAsync(Notification notification);
    Task MarkAsReadAsync(int notificationId);
    Task MarkAllAsReadAsync(int userId);
    Task DeleteNotificationAsync(int notificationId);
    Task SendRealTimeNotificationAsync(int userId, Notification notification);
}