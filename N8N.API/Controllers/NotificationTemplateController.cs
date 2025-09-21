using AutoMapper;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using N8N.API.Context.Entities;
using N8N.API.Models;
using N8N.API.Models.Filters;
using N8N.API.Services.NotificationTemplate;


namespace N8N.API.Controllers
{
    [ApiController]
    [Route("api/notificationtemplates")]
    public class NotificationTemplateController : ControllerBase
    {
       private readonly INotificationTemplateService _notificationTemplateService; 
       private readonly IMapper _mapper;
       public NotificationTemplateController(INotificationTemplateService notificationTemplateService, 
                                             IMapper mapper)
        {
            _notificationTemplateService = notificationTemplateService;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetNotificationTemplates(NotificationTemplateFilter filters)
        {
            var notificationTemplateList = await _notificationTemplateService.GetNotificationTemplatesAsync(filters);
            return Ok(_mapper.Map<IEnumerable<NotificationTemplateDto>>(notificationTemplateList));
        }

        [HttpGet("{templateId}", Name = "GetNotificationTemplate")]
        public async Task<IActionResult> GetNotificationTemplate(Guid templateId)
        {
            if(templateId == Guid.Empty) return BadRequest("templatedId not provided");
            var template = await _notificationTemplateService.GetNotificationTemplateAsync(templateId);
            return Ok(_mapper.Map<NotificationTemplateDto>(template));
        }

        [HttpPost]
        public async Task<IActionResult> AddNotificationTemplate(CreateNotificationTemplateDto template)
        {
            if (template == null) return BadRequest("template not provided");

            var newTemplate = _mapper.Map<NotificationTemplate>(template);
            await _notificationTemplateService.AddNotificationTemplateAsync(newTemplate);
            var templateResponse = _mapper.Map<NotificationTemplateDto>(newTemplate);
            return CreatedAtRoute("GetNotificationTemplate", new { templateId = templateResponse.TemplateId }, templateResponse);
        }

        [HttpDelete("{templateId}")]
        public async Task<IActionResult> RemoveNotificationTemplate(Guid templateId)
        {
            if (templateId == Guid.Empty) return BadRequest("templatedId not provided");

            var templateFound = await _notificationTemplateService.GetNotificationTemplateAsync(templateId);
            if(templateFound == null) return NotFound("notification template not found");
            _notificationTemplateService.RemoveNotificationTemplate(templateFound);
            return NoContent();
        }
    }
}
