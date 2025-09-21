using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using N8N.API.Context.Entities;
using N8N.API.Models;
using N8N.API.Services;
using N8N.API.Services.Event;
using N8N.API.Services.Subscription;

namespace N8N.API.Controllers
{
    [ApiController]
    [Route("api/users/{userId}/events/{eventId}/subscriptions")]
    public class SubscriptionController: ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IEventService _eventService;
        private readonly ISubscriptionService _subscriptionService;
        private readonly IMapper _mapper;
        public SubscriptionController(IUserService userService, 
                                      IEventService eventService, 
                                      ISubscriptionService subscriptionService, 
                                      IMapper mapper) { 
            _userService = userService;
            _eventService = eventService;
            _subscriptionService = subscriptionService;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetSubscriptions(Guid userId, Guid eventId)
        {
            if (userId == Guid.Empty) return BadRequest("userId not provided");
            if (eventId == Guid.Empty) return BadRequest("eventId not provided");

            var subscriptionList = await _subscriptionService.GetSubscriptionsAsync(userId, eventId);
            return Ok(_mapper.Map<IEnumerable<SubscriptionDto>>(subscriptionList));
        }

        [HttpGet("{subscriptionId}", Name = "GetSubscription")]
        public async Task<IActionResult> GetSubscription(Guid userId, Guid eventId, Guid subscriptionId)
        {
            if (userId == Guid.Empty) return BadRequest("userId not provided");
            if (eventId == Guid.Empty) return BadRequest("eventId not provided");
            if (subscriptionId == Guid.Empty) return BadRequest("subscriptionId not provided");

            var subscriptionFound = await _subscriptionService.GetSubscriptionAsync(userId, eventId, subscriptionId);
            if (subscriptionFound == null) return NotFound("subscription not found");
            return Ok(_mapper.Map<SubscriptionDto>(subscriptionFound));
        }

        [HttpPost]
        public async Task<IActionResult> AddSubscription(Guid userId, Guid eventId, CreateSubscriptionDto subscription)
        {
            if (userId == Guid.Empty) return BadRequest("userId not provided");
            if (eventId == Guid.Empty) return BadRequest("eventId not provided");
            if (subscription == null) return BadRequest("subscription not provided");
            if (!await _userService.IsUserExistsAsync(userId)) return NotFound("user not found");
            if (!await _eventService.IsEventExistsAsync(userId, eventId)) return NotFound("event not found");

            var newSubscription = _mapper.Map<Subscription>(subscription);
            await _subscriptionService.AddSubscriptionAsync(newSubscription);
            var subscriptionResponse = _mapper.Map<SubscriptionDto>(newSubscription);

            return CreatedAtRoute("GetSubscription", new { userId, eventId, newSubscription.SubscriptionId }, subscriptionResponse);
        }

        [HttpDelete("{subscriptionId}")]
        public async Task<IActionResult> DeleteSubscription(Guid userId, Guid eventId, Guid subscriptionId)
        {
            if (userId == Guid.Empty) return BadRequest("userId not provided");
            if (eventId == Guid.Empty) return BadRequest("eventId not provided");
            if (subscriptionId == Guid.Empty) return BadRequest("subscriptionId not provided");
            if(!await _userService.IsUserExistsAsync(userId)) return NotFound("user not found");
            if(!await _eventService.IsEventExistsAsync(userId, eventId)) return NotFound("event not found");

            var subscriptionFound = await _subscriptionService.GetSubscriptionAsync(userId, eventId, subscriptionId);
            if (subscriptionFound == null) return NotFound("subscription not found");

            _subscriptionService.RemoveSubscription(subscriptionFound);
            return NoContent();
        }
    }
}
