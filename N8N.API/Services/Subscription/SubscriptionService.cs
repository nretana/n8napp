


using Microsoft.EntityFrameworkCore;
using N8N.API.Context;
using N8N.API.Context.Entities;

namespace N8N.API.Services.Subscription
{
    public class SubscriptionService : ISubscriptionService
    {
        private readonly CalendarDbContext _context;
        public SubscriptionService(CalendarDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Context.Entities.Subscription>> GetSubscriptionsAsync(Guid userId, Guid eventId)
        {
            if (userId == Guid.Empty) throw new ArgumentNullException(nameof(userId));
            if (eventId == Guid.Empty) throw new ArgumentNullException(nameof(eventId));

            var subscriptionList = await _context.Subscriptions.Where(sub => sub.UserId == userId && sub.EventId == eventId).ToListAsync();
            return subscriptionList;
        }

        public async Task<Context.Entities.Subscription?> GetSubscriptionAsync(Guid userId, Guid eventId, Guid subscriptionId)
        {
            if (userId == Guid.Empty) throw new ArgumentNullException(nameof(userId));
            if (eventId == Guid.Empty) throw new ArgumentNullException(nameof(eventId));
            if (subscriptionId == Guid.Empty) throw new ArgumentNullException(nameof(subscriptionId));

            var subscriptionFound = await _context.Subscriptions.FirstOrDefaultAsync(sub => sub.UserId == userId && sub.EventId == eventId && sub.SubscriptionId == subscriptionId);
            return subscriptionFound;
        }

        public async Task AddSubscriptionAsync(Context.Entities.Subscription subscription)
        {
            if (subscription == null) throw new ArgumentNullException(nameof(subscription));

            subscription.SubscriptionId = Guid.NewGuid();
            await _context.Subscriptions.AddAsync(subscription);
            await _context.SaveChangesAsync();
        }

        public void RemoveSubscription(Context.Entities.Subscription subscription)
        {
            if (subscription == null) throw new ArgumentNullException(nameof(subscription));

            _context.Subscriptions.Remove(subscription);
            _context.SaveChanges();
        }

        public async Task<bool> IsSubscriptionExistsAsync(Guid userId, Guid eventId, Guid subscriptionId)
        {
            if (userId == Guid.Empty) throw new ArgumentNullException(nameof(userId));
            if (eventId == Guid.Empty) throw new ArgumentNullException(nameof(eventId));
            if (subscriptionId == Guid.Empty) throw new ArgumentNullException(nameof(subscriptionId));

            var subscriptionFound = await _context.Subscriptions.FirstOrDefaultAsync(sub => sub.UserId == userId && sub.EventId == eventId && sub.SubscriptionId == subscriptionId);
            return subscriptionFound != null;
        }
    }
}
