
using Microsoft.Extensions.Options;
using N8N.API.Configuration;
using N8N.API.Context.Entities;
using N8N.API.Services.Notification;
using N8N.Shared.Services;

namespace N8N.API.Services.Jobs
{
    public class NotificationBackgroundService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly PeriodicTimer _intervalTime;
        private readonly IOptions<NotificationConfig> _options;
        private const double IntervalTimeInMinutesDefault = 2;
        public NotificationBackgroundService(IServiceScopeFactory scopeFactory, IOptions<NotificationConfig> options)
        {
            _scopeFactory = scopeFactory;
            _options = options;
            _intervalTime = new (TimeSpan.FromMinutes(_options.Value?.PollingInterval ?? IntervalTimeInMinutesDefault));
        }

        protected override async Task<Task> ExecuteAsync(CancellationToken stoppingToken)
        {
            stoppingToken.ThrowIfCancellationRequested();
            
            using (var scope = _scopeFactory.CreateScope())
            {
                var sendNotificationService = scope.ServiceProvider.GetRequiredService<ISendNotificationService>();
                while (await _intervalTime.WaitForNextTickAsync(stoppingToken))
                {
                    try
                    {
                        Console.WriteLine($"Process notifications: {DateTime.Now.ToString()}");
                        await sendNotificationService.ProcessNotificationsAsync();
                    }
                    catch (OperationCanceledException ex)
                    {
                        break;
                    }
                    catch (Exception ex) { 
                        //add logs
                    }
                }
            }
            return Task.CompletedTask;
        }

        public override void Dispose()
        {
            _intervalTime.Dispose();
            base.Dispose();
        }
    }
}
