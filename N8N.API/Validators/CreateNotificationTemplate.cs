using FluentValidation;
using N8N.API.Models;
using System.Data;

namespace N8N.API.Validators
{
    public class CreateNotificationTemplate : AbstractValidator<CreateNotificationTemplateDto>
    {
        public CreateNotificationTemplate() {
            RuleFor(t => t.Name).NotEmpty().WithMessage("Name is required");
            RuleFor(t => t.SubjectName).NotEmpty().WithMessage("Subject name is required");
            RuleFor(t => t.BodyTemplate).NotEmpty().WithMessage("Html template is required");   
        }
    }
}
 
