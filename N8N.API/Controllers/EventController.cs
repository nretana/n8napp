
using AutoMapper;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using N8N.API.Context.Entities;
using N8N.API.Models;
using N8N.API.Services;
using N8N.API.Services.Event;
using N8N.API.Configuration;
using N8N.Shared.Services;
using Azure;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.Extensions.Logging;
using N8N.API.ExceptionHandlers.Exceptions;


namespace N8N.API.Controllers
{
    [ApiController]
    [Route("api/users/{userId}/events")]
    public class EventController : ControllerBase
    {
        private readonly IEventService _eventService;
        private readonly IUserService _userService;
        private readonly IMapper _mapper;
        public EventController(IEventService eventService, 
                               IUserService userService, 
                               IMapper mapper,
                               IOptions<N8NConfig> options) {
            _eventService = eventService;
            _userService = userService;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetEvents(Guid userId) {

            if (userId == Guid.Empty) throw new BadRequestException("user id not provided");

            var eventList = await _eventService.GetAllEventsAsync(userId);
            return Ok(_mapper.Map<IEnumerable<EventDto>>(eventList));
        }

        [HttpGet("{eventId}", Name = "GetEvent")]
        public async Task<IActionResult> GetEvent(Guid userId, Guid eventId)
        {
            if(userId == Guid.Empty) throw new BadRequestException("userd not provided");
            if (eventId == Guid.Empty) throw new BadRequestException("event id not provided");
            if (!await _userService.IsUserExistsAsync(userId)) throw new NotFoundException("user not found");
           
            var eventFound = await _eventService.GetEventAsync(userId, eventId);
            if(eventFound is null) return NotFound();
            return Ok(_mapper.Map<EventDto>(eventFound));
        }


        [HttpPost]
        public async Task<IActionResult> AddEvent(Guid userId, CreateEventDto @event)
        {
            if (userId == Guid.Empty) throw new BadRequestException("User id not provided");
            if (@event is null) throw new BadRequestException("Event not provided");
            if (!await _userService.IsUserExistsAsync(userId)) throw new NotFoundException("User not found");
            
            var newEvent = _mapper.Map<Event>(@event);
            await _eventService.AddEventAsync(userId, newEvent);
            var eventResponse = _mapper.Map<EventDto>(newEvent);

            return CreatedAtRoute("GetEvent", new { userId, eventId = newEvent.EventId }, eventResponse);
        }

        [HttpPatch("{eventId}")]
        public async Task<IActionResult> UpdateEvent(Guid userId, Guid eventId, JsonPatchDocument<CreateEventDto> eventPatchDocument)
        {
            if (userId == Guid.Empty) throw new BadRequestException("User id not provided");
            if (eventPatchDocument is null) throw new BadRequestException("Event object not provided");
            if (!await _userService.IsUserExistsAsync(userId)) throw new NotFoundException("User not found");

            var eventFound = await _eventService.GetEventAsync(userId, eventId);
            if (eventFound is null) return NotFound();

            var eventToPatch = _mapper.Map<CreateEventDto>(eventFound);
            eventPatchDocument.ApplyTo(eventToPatch, ModelState);
            if (!ModelState.IsValid) return BadRequest();

            _mapper.Map(eventToPatch, eventFound);
            await _eventService.UpdateEventAsync(userId, eventFound);
            return NoContent();
        }

        [HttpDelete("{eventId}")]
        public async Task<IActionResult> RemoveEvent(Guid userId, Guid eventId)
        {
            if (userId == Guid.Empty) throw new BadRequestException("User id not provided");
            if (eventId == Guid.Empty) throw new BadRequestException("Event id not provided");
            if (!await _userService.IsUserExistsAsync(userId)) throw new NotFoundException("User not found");
            
            var eventFound = await _eventService.GetEventAsync(userId, eventId);
            if (eventFound is null) throw new NotFoundException("Event not found");

            await _eventService.RemovedEventAsync( userId, eventFound);
            return NoContent();
        }
    }
}
