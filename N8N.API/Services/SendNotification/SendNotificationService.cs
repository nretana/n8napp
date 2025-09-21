using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using N8N.API.Configuration;
using N8N.API.Context;
using N8N.API.Context.Entities;
using N8N.API.Models.Jobs;
using N8N.API.Services.Notification;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace N8N.Shared.Services
{
    public class SendNotificationService : ISendNotificationService
    {
        private readonly HttpClient _httpClient;
        private readonly JsonSerializerOptions _serializerOptions;
        private readonly IOptions<N8NConfig> _options;
        private readonly CalendarDbContext _context;
        private readonly INotificationService _notificationService;
        public SendNotificationService(CalendarDbContext context, 
                                       HttpClient httpClient, 
                                       IOptions<N8NConfig> options,
                                       INotificationService notificationService) {
            _context = context;
            _options = options;
            _notificationService = notificationService;
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri(_options.Value.BaseUrl);
            _httpClient.Timeout = new TimeSpan(0, 0, 30);
            _httpClient.DefaultRequestHeaders.Clear();
            _serializerOptions = new JsonSerializerOptions() { 
                PropertyNameCaseInsensitive = true,
                ReferenceHandler = ReferenceHandler.Preserve,
                WriteIndented = true
            };
        }

        public async Task<IEnumerable<Notification>> GetPendingNotificationsAsync()
        {
            var collection = _context.Notifications.AsNoTracking()
                                                   .Include(u => u.User)
                                                   .Include(e => e.Event)
                                                   .Include(t => t.Template)
                                                   .Where(n => n.Status == "Pending" && n.ScheduledTime <= DateTimeOffset.Now);
            return await collection.ToListAsync();
        }

        private async Task<SendNotificationResponse?> SendNotificationAsync(Notification notificationRequest)
        {
            if (notificationRequest is null) throw new ArgumentNullException("notification object not found");
            var notificationContent = JsonSerializer.Serialize(notificationRequest, _serializerOptions);
            var stringContent = new StringContent(notificationContent, Encoding.UTF8, "application/json");
            using var response = await _httpClient.PostAsync(_options.Value.WebHooks["SendNotification"], stringContent);
            response.EnsureSuccessStatusCode();
            var responseContent = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<SendNotificationResponse?>(responseContent);
        }

        public async Task ProcessNotificationsAsync()
        {
            try
            {
                await _context.Database.BeginTransactionAsync();
                var pendingNotificationList = await GetPendingNotificationsAsync();
                foreach (var pendingNotification in pendingNotificationList)
                {
                    var response = await SendNotificationAsync(pendingNotification);
                    var notificationFound = await _notificationService.GetNotificationAsync(pendingNotification.User.UserId, pendingNotification.NotificationId);
                    if (notificationFound == null) throw new NullReferenceException(nameof(notificationFound));

                    notificationFound.Status = response?.IsSuccess == true ? "Sent" : "Failed";
                    notificationFound.SentAt = DateTimeOffset.Now;
                    await _notificationService.SaveChangesAsync();
                }
                await _context.Database.CommitTransactionAsync();
            }
            catch(Exception ex)
            {
                await _context.Database.RollbackTransactionAsync();
                throw ex;
            }

        }

    }
}
