using FluentValidation;
using N8N.API.Models;

namespace N8N.API.Validators
{
    public class CreateUserValidator : AbstractValidator<CreateUserDto>
    {
        public CreateUserValidator() { 
        
            RuleFor(u => u.Email).NotEmpty().WithMessage("Email address is required")
                                 .EmailAddress().WithMessage("Email address is invalid");
        }
    }
}
