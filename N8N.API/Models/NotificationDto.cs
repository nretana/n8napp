using N8N.API.Context;

namespace N8N.API.Models
{
    public class NotificationDto
    {
        public Guid NotificationId { get; set; }

        public Guid EventId { get; set; }

        public Guid UserId { get; set; }

        public Guid TemplateId { get; set; }

        public string EventType { get; set; }

        public DateTimeOffset ScheduledTime { get; set; }

        public DateTimeOffset SentAt { get; set; }

        public DateTimeOffset CreatedAt { get; set; }

    }
}
