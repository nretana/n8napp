using N8N.API.Context;
using System.ComponentModel.DataAnnotations;

namespace N8N.API.Models
{
    public class EventDto
    {
        public Guid EventId { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public DateOnly StartDate { get; set; }

        public DateOnly EndDate { get; set; }

        public TimeOnly StartTime { get; set; }

        public TimeOnly EndTime { get; set; }

        public Guid OrganizerId { get; set; }

        public DateTimeOffset CreatedAt { get; set; }

        public DateTimeOffset ModifiedAt { get; set; }

    }
}
