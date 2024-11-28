using AllOfTheHackathon.Contracts;
using AllOfTheHackathon.Database.Entity;
using AutoMapper;

namespace AllOfTheHackathon.Mapper;

public class TeamProfile : Profile
{
    public TeamProfile()
    {
        CreateMap<Team, TeamEntity>().ForMember(
            dest => dest.Junior,
            opt => opt.MapFrom(src => new JuniorEntity { Id = src.Junior.Id }));
        CreateMap<Team, TeamEntity>().ForMember(
            dest => dest.TeamLead,
            opt => opt.MapFrom(src => new TeamLeadEntity { Id = src.TeamLead.Id }));

        CreateMap<Employee, JuniorEntity>();
        CreateMap<Employee, TeamLeadEntity>();
    }
}