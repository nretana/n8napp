
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using N8N.API.Context;
using N8N.API.Context.Entities;
using N8N.API.EnumTypes;
using N8N.API.Models;
using N8N.API.Services.Jobs;
using N8N.API.Services.Notification;
using N8N.API.Services.NotificationTemplate;

namespace N8N.API.Services.Event
{
    public class EventService : IEventService
    {
        private readonly CalendarDbContext _context;
        private readonly IUserService _userService;
        private readonly INotificationService _notificationService;
        private readonly INotificationTemplateService _notificationTemplateService;
        public EventService(CalendarDbContext context, 
                            IUserService userService, 
                            INotificationService notificationService, 
                            INotificationTemplateService notificationTemplateService)
        {
            _context = context;
            _userService = userService;
            _notificationService = notificationService;
            _notificationTemplateService = notificationTemplateService;
        }

        public async Task<IEnumerable<Context.Entities.Event>> GetAllEventsAsync(Guid userId)
        {
            if (userId == Guid.Empty) throw new ArgumentNullException(nameof(userId));
            var eventList = await _context.Events.Where(@event => @event.OrganizerId == userId).ToListAsync();
            return eventList;
        }

        public async Task<Context.Entities.Event?> GetEventAsync(Guid userId, Guid eventId)
        {
            if (userId == Guid.Empty) throw new ArgumentNullException(nameof(userId));
            if (eventId == Guid.Empty) throw new ArgumentNullException(nameof(eventId));

            var eventFound = await _context.Events.FirstOrDefaultAsync(@event => @event.OrganizerId == userId 
                        && @event.EventId == eventId);
            return eventFound;
        }

        public async Task AddEventAsync(Guid userId, Context.Entities.Event @event)
        {
            if (userId == Guid.Empty) throw new ArgumentNullException(nameof(userId));
            if (@event == null) throw new ArgumentNullException(nameof(@event));
            try
            {
                await _context.Database.BeginTransactionAsync();
                @event.EventId = Guid.NewGuid();
                @event.OrganizerId = userId;
                await _context.Events.AddAsync(@event);
                await _context.SaveChangesAsync();

                var userFound = await _userService.GetUserAsync(userId);
                if (userFound == null) throw new NullReferenceException(nameof(userFound));

                var templateFound = (await _notificationTemplateService.GetNotificationTemplatesAsync(
                    new Models.Filters.NotificationTemplateFilter() { Name = "add_event_template" })).FirstOrDefault();
                if (templateFound == null) throw new NullReferenceException(nameof(templateFound));

                var eventNotification = new Context.Entities.Notification()
                {
                    EventId = @event.EventId,
                    UserId = userId,
                    TemplateId = templateFound.TemplateId,
                    EventType = EventType.AddEvent.ToString(),
                    ScheduledTime = DateTimeOffset.Now,
                    Status = NotificationStatus.Pending.ToString()
                };

                await _notificationService.AddNotificationAsync(eventNotification);
                await _context.Database.CommitTransactionAsync();
            }
            catch (Exception ex)
            {
                await _context.Database.RollbackTransactionAsync();
                throw ex;
            }
        }

        public async Task UpdateEventAsync(Guid userId, Context.Entities.Event @event)
        {
            if (userId == Guid.Empty) throw new ArgumentNullException(nameof(userId));
            try
            {
                await _context.Database.BeginTransactionAsync();
                _context.Events.Update(@event);

                var templateFound = (await _notificationTemplateService.GetNotificationTemplatesAsync(
                    new Models.Filters.NotificationTemplateFilter() { Name = "update_event_template" })).FirstOrDefault();
                if(templateFound == null) throw new NullReferenceException(nameof(templateFound));

                await UpdateCurrentNotification(userId, @event);

                var notification = new Context.Entities.Notification()
                {
                    EventId = @event.EventId,
                    UserId = userId,
                    TemplateId = templateFound.TemplateId,
                    EventType = EventType.UpdateEvent.ToString(),
                    ScheduledTime = DateTimeOffset.Now,
                    Status = NotificationStatus.Pending.ToString()
                };

                await _notificationService.AddNotificationAsync(notification);
                await _context.Database.CommitTransactionAsync();
            }
            catch (Exception ex)
            {
                await _context.Database.RollbackTransactionAsync();
                throw ex;
            }
        }

        public async Task RemovedEventAsync(Guid userId, Context.Entities.Event @event)
        {
            if (@event == null) throw new ArgumentNullException(nameof(@event));

            try
            {
                await _context.Database.BeginTransactionAsync();
                @event.IsDeleted = true;
                _context.Events.Update(@event);

                await UpdateCurrentNotification(userId, @event);

                var templateFound = (await _notificationTemplateService.GetNotificationTemplatesAsync(
                    new Models.Filters.NotificationTemplateFilter() { Name = "cancel_event_template" })).FirstOrDefault();
                if (templateFound == null) throw new NullReferenceException(nameof(templateFound));

                var notification = new Context.Entities.Notification()
                {
                    EventId = @event.EventId,
                    UserId = userId,
                    TemplateId = templateFound.TemplateId,
                    EventType = EventType.CancelEvent.ToString(),
                    ScheduledTime = DateTimeOffset.Now,
                    Status = NotificationStatus.Pending.ToString()
                };

                await _notificationService.AddNotificationAsync(notification);
                await _context.SaveChangesAsync();
                await _context.Database.CommitTransactionAsync();
            }
            catch(Exception ex)
            {
                await _context.Database.RollbackTransactionAsync();
                throw ex;
            }
        }

        public async Task<bool> IsEventExistsAsync(Guid userId,  Guid eventId)
        {
            if (userId == Guid.Empty) throw new ArgumentNullException(nameof(userId));
            if (eventId == Guid.Empty) throw new ArgumentNullException(nameof(eventId));

            var eventFound = await _context.Events.FirstOrDefaultAsync(@event => @event.OrganizerId == userId && @event.EventId == eventId);
            return eventFound != null;
        }

        private async Task UpdateCurrentNotification(Guid userId, Context.Entities.Event @event)
        {
            var eventNotifications = await _context.Notifications.Where(
                                    n => n.UserId == userId
                                    && (n.EventId == @event.EventId && n.EventType == EventType.AddEvent.ToString())
                                    || (n.EventId == @event.EventId && n.EventType == EventType.UpdateEvent.ToString())
                                    && n.Status == NotificationStatus.Pending.ToString()).ToListAsync();
            if (eventNotifications != null)
            {
                foreach (var notification in eventNotifications)
                {
                    notification.Status = NotificationStatus.Cancelled.ToString();
                }
            }
        }
    }
}
