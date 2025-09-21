using AutoMapper;
using N8N.API.Context.Entities;
using N8N.API.Models;

namespace N8N.API.Mappers
{
    public class NotificationProfile : Profile
    {
        public NotificationProfile() {

            CreateMap<Notification, NotificationDto>().ReverseMap();
            CreateMap<Notification, CreateNotificationDto>().ReverseMap();
            CreateMap<CreateNotificationDto, NotificationDto>();
        }
    }
}
