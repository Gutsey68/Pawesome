using Pawesome.Models.Entities;

namespace Pawesome.Interfaces;

public interface INotificationRepository
{
    Task<IEnumerable<Notification>> GetNotificationsForUserAsync(int userId, int skip = 0, int take = 20);
    Task<int> GetUnreadCountAsync(int userId);
    Task<Notification> CreateAsync(Notification notification);
    Task MarkAsReadAsync(int notificationId);
    Task MarkAllAsReadAsync(int userId);
    Task DeleteAsync(int notificationId);
}