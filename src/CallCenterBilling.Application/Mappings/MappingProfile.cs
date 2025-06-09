using AutoMapper;
using CallCenterBilling.Application.DTOs;
using CallCenterBilling.Domain.Entities;

namespace CallCenterBilling.Application.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<ApplicationUser, UserDto>()
            .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.FirstName));
        CreateMap<UserDto, ApplicationUser>();
    }
}