using N8N.API.Context;

namespace N8N.API.Models
{
    public class SubscriptionDto
    {
        public Guid SubscriptionId { get; set; }

        public Guid UserId { get; set; }

        public Guid EventId { get; set; }

        public DateTimeOffset SubscribedAt { get; set; }

        public bool IsActive { get; set; } = true;
    }
}
