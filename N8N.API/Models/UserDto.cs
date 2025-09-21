using N8N.API.Context;

namespace N8N.API.Models
{
    public class UserDto
    {
        public Guid UserId { get; set; }

        public string Email { get; set; }

        public string Timezone { get; set; }

        public bool IsActive { get; set; }

        public DateTimeOffset CreatedAt { get; set; }

        public DateTimeOffset ModifiedAt { get; set; }

    }
}
