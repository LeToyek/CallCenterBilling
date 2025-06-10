using AutoMapper;
using CallCenterBilling.Application.DTOs;
using CallCenterBilling.Domain.Entities;

namespace CallCenterBilling.Application.Mappings
{
    public class AgentMappingProfile : Profile
    {
        public AgentMappingProfile()
        {
            CreateMap<Agent, AgentDto>()
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()));
            
            CreateMap<CreateAgentDto, Agent>()
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => AgentStatus.Offline));
        }
    }
}