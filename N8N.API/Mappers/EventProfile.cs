using AutoMapper;
using N8N.API.Context.Entities;
using N8N.API.Models;

namespace N8N.API.Mappers
{
    public class EventProfile : Profile
    {
        public EventProfile() {
            CreateMap<Event, EventDto>().ReverseMap();
            CreateMap<Event, CreateEventDto>().ReverseMap();
            CreateMap<CreateEventDto, EventDto>();
        }
    }
}
