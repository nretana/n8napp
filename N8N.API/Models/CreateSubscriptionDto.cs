using N8N.API.Context;

namespace N8N.API.Models
{
    public class CreateSubscriptionDto
    {

        public Guid UserId { get; set; }

        public Guid EventId { get; set; }

        public bool IsActive { get; set; }

    }
}
