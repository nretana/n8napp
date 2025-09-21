using N8N.API.Context;
using System.ComponentModel.DataAnnotations;

namespace N8N.API.Models
{
    public class CreateNotificationTemplateDto
    {
        public string Name { get; set; }

        public string SubjectName { get; set; }

        public string BodyTemplate { get; set; }

    }
}
