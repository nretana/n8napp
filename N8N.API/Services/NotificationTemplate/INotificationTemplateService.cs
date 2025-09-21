using N8N.API.Models.Filters;

namespace N8N.API.Services.NotificationTemplate
{
    public interface INotificationTemplateService
    {
        public Task<IEnumerable<Context.Entities.NotificationTemplate>> GetNotificationTemplatesAsync(NotificationTemplateFilter filters);

        public Task<Context.Entities.NotificationTemplate?> GetNotificationTemplateAsync(Guid notificationTemplateId);

        public Task AddNotificationTemplateAsync(Context.Entities.NotificationTemplate notificationTemplate);

        public void RemoveNotificationTemplate(Context.Entities.NotificationTemplate notificationTemplate);
    }
}
