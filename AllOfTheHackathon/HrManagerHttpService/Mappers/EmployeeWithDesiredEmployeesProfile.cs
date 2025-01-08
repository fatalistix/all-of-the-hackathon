using System.Text.Json;
using AllOfTheHackathon.Contracts;
using AutoMapper;
using HrManagerHttpService.Databases.Entities;
using HrManagerHttpService.Models;

namespace HrManagerHttpService.Mappers;

public class EmployeeWithDesiredEmployeesProfile : Profile
{
    public EmployeeWithDesiredEmployeesProfile()
    {
        CreateMap<JuniorWithDesiredTeamLeadsIds, EmployeeWithDesiredEmployees>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
            .ForMember(dest => dest.DesiredEmployees,
                opt => opt.MapFrom(src => JsonStringToIListIntConverter.Convert(src.DesiredTeamLeadsIds)));

        CreateMap<TeamLeadWithDesiredJuniorsIds, EmployeeWithDesiredEmployees>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
            .ForMember(dest => dest.DesiredEmployees,
                opt => opt.MapFrom(src => JsonStringToIListIntConverter.Convert(src.DesiredJuniorsIds)));

        CreateMap<EmployeeWithDesiredEmployees, Employee>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name));
    }
}

internal static class JsonStringToIListIntConverter
{
    public static IList<int> Convert(string sourceMember)
    {
        return JsonSerializer.Deserialize<IList<int>>(sourceMember)!;
    }
}