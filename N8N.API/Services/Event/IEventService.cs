using Entities = N8N.API.Context;

namespace N8N.API.Services.Event
{
    public interface IEventService
    {
        public Task<IEnumerable<Context.Entities.Event>> GetAllEventsAsync(Guid userId);

        public Task<Context.Entities.Event?> GetEventAsync(Guid userId, Guid eventId);

        public Task AddEventAsync(Guid userId, Context.Entities.Event @event);

        public Task UpdateEventAsync(Guid userId, Context.Entities.Event @event);

        public Task RemovedEventAsync(Guid userId, Context.Entities.Event @event);

        public Task<bool> IsEventExistsAsync(Guid userId, Guid eventId);
    }
}
