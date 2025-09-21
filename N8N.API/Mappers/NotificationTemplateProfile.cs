using AutoMapper;
using N8N.API.Context.Entities;
using N8N.API.Models;

namespace N8N.API.Mappers
{
    public class NotificationTemplateProfile : Profile
    {
        public NotificationTemplateProfile()
        {
            CreateMap<NotificationTemplate, NotificationTemplateDto>().ReverseMap();
            CreateMap<NotificationTemplate, CreateNotificationTemplateDto>().ReverseMap();
        }
    }
}
