
using Microsoft.EntityFrameworkCore;
using N8N.API.Context;
using N8N.API.Models.Filters;

namespace N8N.API.Services.NotificationTemplate
{
    public class NotificationTemplateService : INotificationTemplateService
    {
        private readonly CalendarDbContext _context;
        public NotificationTemplateService(CalendarDbContext context) { 
            _context = context;
        }

        public async Task<IEnumerable<Context.Entities.NotificationTemplate>> GetNotificationTemplatesAsync(NotificationTemplateFilter filters)
        {
            var collection = _context.NotificationTemplates as IQueryable<Context.Entities.NotificationTemplate>;
            if (!string.IsNullOrEmpty(filters.Name))
            {
                collection = collection.Where(t =>  t.Name == filters.Name);
            }

            var notificationTemplates = await collection.ToListAsync();
            return notificationTemplates;
        }

        public async Task<Context.Entities.NotificationTemplate?> GetNotificationTemplateAsync(Guid templateId)
        {
            if (templateId == Guid.Empty) throw new ArgumentNullException(nameof(templateId));

            var notificationTemplateFound = await _context.NotificationTemplates.FirstOrDefaultAsync(notificationTemplate => notificationTemplate.TemplateId == templateId);
            return notificationTemplateFound;
        }

        public async Task AddNotificationTemplateAsync(Context.Entities.NotificationTemplate template)
        {
            if (template == null) throw new ArgumentNullException(nameof(template));

            template.TemplateId = Guid.NewGuid();
            await _context.NotificationTemplates.AddAsync(template);
            await _context.SaveChangesAsync();
        }

        public void RemoveNotificationTemplate(Context.Entities.NotificationTemplate template)
        {
            if (template == null) throw new ArgumentNullException(nameof(template));

            _context.NotificationTemplates.Remove(template);
            _context.SaveChanges();
        }
    }
}
