using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using N8N.API.Context.Entities;
using N8N.API.Models;
using N8N.API.Services;
using N8N.API.Services.Notification;

namespace N8N.API.Controllers
{
    [ApiController]
    [Route("api/users/{userId}/notifications")]
    public class NotificationController : ControllerBase
    {
        private readonly INotificationService _notificationService;
        private readonly IUserService _userService;
        private readonly IMapper _mapper;
        public NotificationController(IUserService userService, INotificationService notificationService, IMapper mapper) { 
            _userService = userService;
            _notificationService = notificationService;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetNotifications(Guid userId)
        {
            if (userId == Guid.Empty) return BadRequest("userId not provided");

            var notificationList = await _notificationService.GetNotificationsAsync(userId);
            return Ok(_mapper.Map<IEnumerable<NotificationDto>>(notificationList));
        }

        [HttpGet("{notificationId}", Name = "GetNotification")]
        public async Task<IActionResult> GetNotification(Guid userId, Guid notificationId)
        {
            if (userId == Guid.Empty) return BadRequest("userId not provided");
            if (notificationId == Guid.Empty) return BadRequest("notificationId not provided");

            var notificationFound = await _notificationService.GetNotificationAsync(userId, notificationId);
            if(notificationFound == null) return NotFound("notification not found");

            return Ok(_mapper.Map<NotificationDto>(notificationFound));
        }

        [HttpPost]
        public async Task<IActionResult> AddNotification(Guid userId, CreateNotificationDto notification)
        {
            if (userId == Guid.Empty) return BadRequest("userId not provided");
            if (notification == null) return BadRequest("notification not provided");
            if (!await _userService.IsUserExistsAsync(userId)) return NotFound("user not found");

            var newNotification = _mapper.Map<Notification>(notification);
            await _notificationService.AddNotificationAsync(newNotification);
            var notificationResponse = _mapper.Map<NotificationDto>(newNotification);

            return CreatedAtRoute("GetNotification", new { userId, notificationId = newNotification.NotificationId }, notificationResponse);
        }

        [HttpDelete("{notificationId}")]
        public async Task<IActionResult> DeleteNotification(Guid userId, Guid notificationId)
        {
            if (userId == Guid.Empty) return BadRequest("userId not provided");
            if (notificationId == Guid.Empty) return BadRequest("notificationId not provided");
            if (!await _userService.IsUserExistsAsync(userId)) return NotFound("user not found");

            var notificationFound = await _notificationService.GetNotificationAsync(userId, notificationId);
            if (notificationFound == null) return NotFound("notification not found");
            _notificationService.RemoveNotification(notificationFound);
            return NoContent();
        }

        /*[HttpPost]
        public async Task<IActionResult> SentNotification(NotificationDto notification)
        {
            if(notification == null)
            {
                return BadRequest("notification not set");
            }

            var notificationData = _mapper.Map<Notification>(notification);
            //await _notificationService.SendNotification(notificationData);

            return Ok();
        }*/
    }
}
