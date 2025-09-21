using AutoMapper;
using N8N.API.Context.Entities;
using N8N.API.Models;

namespace N8N.API.Mappers
{
    public class SubscriptionProfile : Profile
    {
        public SubscriptionProfile() {
            CreateMap<Subscription, SubscriptionDto>().ReverseMap();
            CreateMap<Subscription, CreateSubscriptionDto>().ReverseMap();
            CreateMap<CreateSubscriptionDto, SubscriptionDto>();
        }
    }
}
