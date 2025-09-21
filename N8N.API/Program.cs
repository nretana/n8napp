using Microsoft.EntityFrameworkCore;
using N8N.API.ActionFilters;
using N8N.API.Configuration;
using N8N.API.Context;
using N8N.API.Services;
using N8N.API.Services.Event;
using N8N.API.Services.Jobs;
using N8N.API.Services.Notification;
using N8N.API.Services.NotificationTemplate;
using N8N.API.Services.Subscription;
using N8N.API.Validators;
using FluentValidation;
using JsonConverters = N8N.API.Utilities.JsonConverters;
using SharedServices = N8N.Shared.Services;
using N8N.API.Middlewares;
using N8N.API.Utilities.Formatters;
using Microsoft.AspNetCore.Http.Features;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOptions<N8NConfig>().BindConfiguration("N8NConfig")
                                                       .ValidateDataAnnotations()
                                                       .ValidateOnStart();
builder.Services.AddOptions<NotificationConfig>().BindConfiguration("NotificationConfig")
                                                       .ValidateDataAnnotations()
                                                       .ValidateOnStart();

builder.Services.AddDbContext<CalendarDbContext>(options => 
options.UseSqlServer(builder.Configuration["ConnectionStrings:CalendarDbConnectionString"]));

// Add services to the container.
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IEventService, EventService>();
builder.Services.AddScoped<ISubscriptionService, SubscriptionService>();
builder.Services.AddScoped<INotificationService, NotificationService>();
builder.Services.AddScoped<INotificationTemplateService, NotificationTemplateService>();
builder.Services.AddHttpClient<SharedServices.ISendNotificationService, SharedServices.SendNotificationService>();

builder.Services.AddHostedService<NotificationBackgroundService>();

builder.Services.AddControllers(options => { 
                    options.Filters.Add<ModelValidationFilter>();
                    options.InputFormatters.Insert(0, JsonPatchInputFormatter.GetJsonPtachInputFormatter());
                })
                .AddJsonOptions(options => {
                    options.JsonSerializerOptions.Converters.Add(new JsonConverters.TimeOnlyConverter());
                    options.JsonSerializerOptions.Converters.Add(new JsonConverters.DateOnlyConverter());
                });


// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddValidatorsFromAssemblyContaining<CreateEventValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<CreateUserValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<CreateNotificationValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<CreateNotificationTemplate>();

builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
builder.Services.AddProblemDetails(options =>
{
    options.CustomizeProblemDetails = context =>
    {
        context.ProblemDetails.Instance = $"{context.HttpContext.Request.Method} {context.HttpContext.Request.Path}";
        context.ProblemDetails.Extensions.TryAdd("requestId", context.HttpContext.TraceIdentifier);
        var activity = context.HttpContext.Features.Get<IHttpActivityFeature>()?.Activity;
        context.ProblemDetails.Extensions.TryAdd("traceId", activity?.Id);
    };
});


var app = builder.Build();

app.UseMiddleware<ExceptionHandlerMiddleware>();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

//app.UseCors();

app.UseAuthorization();

app.MapControllers();

app.Run();
