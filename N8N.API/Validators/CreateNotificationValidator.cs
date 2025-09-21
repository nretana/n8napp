using FluentValidation;
using N8N.API.EnumTypes;
using N8N.API.Models;

namespace N8N.API.Validators
{
    public class CreateNotificationValidator : AbstractValidator<CreateNotificationDto>
    {
        private readonly List<string> eventTypes = new List<string>() { EventType.AddEvent.ToString(), 
                                                                        EventType.UpdateEvent.ToString(), 
                                                                        EventType.CancelEvent.ToString(), 
                                                                        "AddSubscriber", "RemoveSubscriber" };
        public CreateNotificationValidator() {
            RuleFor(n => n.EventId).NotEmpty().WithMessage("event id is required");
            RuleFor(n => n.UserId).NotEmpty().WithMessage("user id is required");
            RuleFor(n => n.TemplateId).NotEmpty().WithMessage("template id is required");
            RuleFor(n => n.ScheduledTime).NotEmpty().WithMessage("Scheduled time is required");
            RuleFor(t => t.EventType).NotEmpty().WithMessage("Event type is required")
                                     .Must(type => eventTypes.Contains(type)).WithMessage("Event Type is not valid");
        }
    }
}
