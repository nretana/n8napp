namespace N8N.API.Services.Subscription
{
    public interface ISubscriptionService
    {
        public Task<IEnumerable<Context.Entities.Subscription>> GetSubscriptionsAsync(Guid userId, Guid eventId);

        public Task<Context.Entities.Subscription?> GetSubscriptionAsync(Guid userId, Guid eventId, Guid subscriptionId);

        public Task AddSubscriptionAsync(Context.Entities.Subscription subscription);

        public void RemoveSubscription(Context.Entities.Subscription subscription);

        public Task<bool> IsSubscriptionExistsAsync(Guid userId, Guid eventId, Guid subscriptionId);
    }
}
