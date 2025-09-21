using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using N8N.API.Models;

namespace N8N.API.ActionFilters
{
    public class ModelValidationFilter : IActionFilter
    {
        private readonly IServiceProvider _serviceProvider;
        public ModelValidationFilter(IServiceProvider serviceProvider) { 
            _serviceProvider = serviceProvider;
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            if (context.ActionArguments.Count == 0) return;

            foreach (var arg in context.ActionArguments.Values)
            {
                if (arg == null) continue;

                var validatorType = typeof(IValidator<>).MakeGenericType(arg.GetType());
                var validator = _serviceProvider.GetService(validatorType) as IValidator;
                if(validator != null)
                {
                    var validatorContext = new ValidationContext<object>(arg);
                    var validatorResult = validator.Validate(validatorContext);
                    if(!validatorResult.IsValid)
                    {
                        foreach (var error in validatorResult.Errors)
                        {
                            context.ModelState.AddModelError(error.PropertyName, error.ErrorMessage);
                        }
                    }
                }
            }

            if(!context.ModelState.IsValid)
            {
                context.Result = new BadRequestObjectResult(context.ModelState);
            }
        }
    }
}
