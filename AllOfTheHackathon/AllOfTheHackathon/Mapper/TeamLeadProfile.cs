using AllOfTheHackathon.Contracts;
using AllOfTheHackathon.Database.Entity;
using AutoMapper;

namespace AllOfTheHackathon.Mapper;

public class TeamLeadProfile : Profile
{
    public TeamLeadProfile()
    {
        CreateMap<Employee, TeamLeadEntity>().ForMember(
            dest => dest.Id, 
            opt => opt.MapFrom(src => src.Id));
        CreateMap<Employee, TeamLeadEntity>().ForMember(
            dest => dest.Name,
            opt => opt.MapFrom(src => src.Name));

        CreateMap<TeamLeadEntity, Employee>().ForMember(
            dest => dest.Id,
            opt => opt.MapFrom(src => src.Id));
        CreateMap<TeamLeadEntity, Employee>().ForMember(
            dest => dest.Name,
            opt => opt.MapFrom(src => src.Name));
    }
}