using FluentValidation;
using N8N.API.Models;

namespace N8N.API.Validators
{
    public class CreateEventValidator : AbstractValidator<CreateEventDto>
    {
        public CreateEventValidator() {

            RuleFor(e => e.Title).NotEmpty().WithMessage("Title is required")
                                 .MaximumLength(200).WithMessage("Title must be short"); ;
            RuleFor(e => e.Description).MaximumLength(400).WithMessage("Description must be short");
            RuleFor(e => e.StartTime).NotEmpty().WithMessage("Start time is required")
                                     .LessThan(e => e.EndTime).WithMessage("Start time must be less than end time");
            RuleFor(e => e.EndTime).NotEmpty().WithMessage("End time is required");
            RuleFor(e => e.StartDate).NotEmpty().WithMessage("Start date is required")
                                     .LessThanOrEqualTo(e => e.EndDate).WithMessage("Start date must be less than end date");
            RuleFor(e => e.EndDate).NotEmpty().WithMessage("End date is required");
        }
    }
}
