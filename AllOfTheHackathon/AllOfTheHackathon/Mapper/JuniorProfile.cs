using AllOfTheHackathon.Contracts;
using AllOfTheHackathon.Database.Entity;
using AutoMapper;

namespace AllOfTheHackathon.Mapper;

public class JuniorProfile : Profile
{
    public JuniorProfile()
    {
        CreateMap<Employee, JuniorEntity>().ForMember(
            dest => dest.Id, 
            opt => opt.MapFrom(src => src.Id));
        CreateMap<Employee, JuniorEntity>().ForMember(
            dest => dest.Name,
            opt => opt.MapFrom(src => src.Name));
        
        CreateMap<JuniorEntity, Employee>().ForMember(
            dest => dest.Id,
            opt => opt.MapFrom(src => src.Id));
        CreateMap<JuniorEntity, Employee>().ForMember(
            dest => dest.Name,
            opt => opt.MapFrom(src => src.Name));
    }
}