using AutoMapper;
using CncApp.Application.Dtos.Users;
using CncApp.Domain.Entities;

namespace CncApp.Application.Mapping;

public class UserProfile : Profile
{
    public UserProfile()
    {
        CreateMap<User, UserDto>();
    }
}

