namespace N8N.API.Models
{
    public class CreateUserDto
    {
        public string FirstName { get; set; }

        public string LastName { get; set; }
        
        public string Email { get; set; }

        public string? Timezone { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; }

        public DateTime ModifiedAt { get; set; }
    }
}
