using Microsoft.AspNetCore.Mvc;
using N8N.API.ExceptionHandlers.Exceptions;
using N8N.API.Models.Errors;
using System.Data;
using System.Net;
using System.Reflection;
using System.Text.Json;

namespace N8N.API.Middlewares
{
    public class ExceptionHandlerMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlerMiddleware> _logger;
        private readonly IProblemDetailsService _problemDetailsService;
        public ExceptionHandlerMiddleware(RequestDelegate requestDelegate, 
                                          ILogger<ExceptionHandlerMiddleware> logger, 
                                          IProblemDetailsService problemDetailsService)
        {
            _next = requestDelegate;
            _logger = logger;
            _problemDetailsService = problemDetailsService;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            try
            {
                Console.WriteLine("EXECUTING MIDDLEWARE!!!!");
                await _next(httpContext);
            }
            catch(Exception ex)
            {
                await HandleExceptionAsync(httpContext, ex);
            }
        }

        public async Task HandleExceptionAsync(HttpContext httpContext, Exception ex)
        {
            var errorDetails = GetErrorDetails(ex);
            httpContext.Response.ContentType = "application/problem+json";
            httpContext.Response.StatusCode = errorDetails?.Status ?? (int)HttpStatusCode.InternalServerError;

            await _problemDetailsService.TryWriteAsync(new ProblemDetailsContext()
            {
                HttpContext = httpContext,
                ProblemDetails = errorDetails
            });
        }

        private ProblemDetails GetErrorDetails(Exception exception)
        {
            var errorDetails = new ProblemDetails()
            {
                Status = (int)HttpStatusCode.InternalServerError,
                Title = "Internal Server Error",
                Detail = exception.Message
            };

            if (exception is ProblemException problemException)
            {

                switch (exception)
                {
                    case BadRequestException:
                        errorDetails.Status = (int)HttpStatusCode.BadRequest;
                        errorDetails.Title = "Bad Request";
                        errorDetails.Detail = problemException.ErrorMessage;
                        break;
                    case NotFoundException:
                        errorDetails.Status = (int)HttpStatusCode.NotFound;
                        errorDetails.Title = "Not Found";
                        errorDetails.Detail = problemException.ErrorMessage;
                        break;
                    case UnauthorizedException:
                        errorDetails.Status = (int)HttpStatusCode.Unauthorized;
                        errorDetails.Title = "Unauthorized";
                        errorDetails.Detail = problemException.ErrorMessage;
                        break;
                        /* case TaskCanceledException:
                             errorDetails.Detail = "Operation was cancelled";
                             break;*/
                }
            }

            return errorDetails;
        }
    }
}
