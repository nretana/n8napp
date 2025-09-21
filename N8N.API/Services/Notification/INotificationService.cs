using N8N.API.Context;

namespace N8N.API.Services.Notification
{
    public interface INotificationService
    {
        public Task<IEnumerable<Context.Entities.Notification>> GetNotificationsAsync(Guid userId);

        public Task<Context.Entities.Notification?> GetNotificationAsync(Guid userId, Guid notificationId);

        public Task AddNotificationAsync(Context.Entities.Notification notification);

        public void RemoveNotification(Context.Entities.Notification notification);

        public Task SaveChangesAsync();
    }
}
