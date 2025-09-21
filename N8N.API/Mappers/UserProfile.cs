using AutoMapper;
using N8N.API.Context.Entities;
using N8N.API.Models;

namespace N8N.API.Mappers
{
    public class UserProfile : Profile
    {
        public UserProfile() { 
            CreateMap<User, UserDto>().ReverseMap();
            CreateMap<User, CreateUserDto>().ReverseMap();
            CreateMap<CreateUserDto, UserDto>();
        }
    }
}
