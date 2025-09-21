using Microsoft.EntityFrameworkCore;
using N8N.API.Context;
using System.Text;
using System.Text.Json;

namespace N8N.API.Services.Notification
{
    public class NotificationService : INotificationService
    {
        private readonly JsonSerializerOptions _jsonSerializerOptions;
        private readonly CalendarDbContext _context;

        public NotificationService(CalendarDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Context.Entities.Notification>> GetNotificationsAsync(Guid userId)
        {
            var notificationList = await _context.Notifications.Where(notification => notification.UserId == userId).ToListAsync();
            return notificationList;
        }

        public async Task<Context.Entities.Notification?> GetNotificationAsync(Guid userId, Guid notificationId)
        {
            if (userId == Guid.Empty) throw new ArgumentNullException(nameof(userId));
            if (notificationId == Guid.Empty) throw new ArgumentNullException(nameof(notificationId));

            var notificationFound = await _context.Notifications.FirstOrDefaultAsync(notification => notification.UserId == userId && notification.NotificationId == notificationId);
            return notificationFound;
        }

        public async Task AddNotificationAsync(Context.Entities.Notification notification)
        {
            if (notification == null) throw new ArgumentNullException(nameof(notification));

            notification.NotificationId = Guid.NewGuid();
            await _context.Notifications.AddAsync(notification);
            await _context.SaveChangesAsync();
        }

        public void RemoveNotification(Context.Entities.Notification notification)
        {
            if (notification == null) throw new ArgumentNullException(nameof(notification));

            _context.Notifications.Remove(notification);
            _context.SaveChanges();
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
