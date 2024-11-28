using AllOfTheHackathon.Contracts;
using AllOfTheHackathon.Database.Entity;
using AutoMapper;

namespace AllOfTheHackathon.Mapper;

public class TeamLeadWishlistProfile : Profile
{
    public TeamLeadWishlistProfile()
    {
        CreateMap<Wishlist, TeamLeadWishlistEntity>().ForMember(
            dest => dest.EmployeeId,
            opt => opt.MapFrom(src => src.EmployeeId));
        CreateMap<Wishlist, TeamLeadWishlistEntity>().ForMember(
            dest => dest.DesiredEmployee,
            opt => opt.MapFrom(src => src.DesiredEmployees));
    }}