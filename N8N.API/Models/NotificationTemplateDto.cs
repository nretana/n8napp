using N8N.API.Context;

namespace N8N.API.Models
{
    public class NotificationTemplateDto
    {
        public Guid TemplateId { get; set; }

        public string Name { get; set; }

        public string SubjectName { get; set; }

        public string BodyTemplate { get; set; }

    }
}
