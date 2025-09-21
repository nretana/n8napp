using N8N.API.Context.Entities;
using N8N.API.Models.Jobs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace N8N.Shared.Services
{
    public interface ISendNotificationService
    {
        public Task<IEnumerable<Notification>> GetPendingNotificationsAsync();

        //public Task<SendNotificationResponse?> SendNotificationAsync(Notification request);

        public Task ProcessNotificationsAsync();
    }
}
