using AllOfTheHackathon.Contracts;
using AllOfTheHackathon.Database.Entity;
using AutoMapper;

namespace AllOfTheHackathon.Mapper;

public class JuniorWishlistProfile : Profile
{
    public JuniorWishlistProfile()
    {
        CreateMap<Wishlist, JuniorWishlistEntity>().ForMember(
            dest => dest.EmployeeId,
            opt => opt.MapFrom(src => src.EmployeeId));
        CreateMap<Wishlist, JuniorWishlistEntity>().ForMember(
            dest => dest.DesiredEmployee,
            opt => opt.MapFrom(src => src.DesiredEmployees));
    }
}