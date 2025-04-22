using AutoMapper;
using Tixora.Core.DTOs;
using Tixora.Core.Entities;

namespace Tixora.Service;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // User mappings
        CreateMap<TbUser, UserResponseDTO>();
        CreateMap<UserRegisterDTO, TbUser>()
            .ForMember(dest => dest.RoleName, opt => opt.MapFrom(src => "user")); // Force role to be user

        // Movie mappings
        CreateMap<TbMovie, MovieResponseDTO>();
        CreateMap<MovieCreateDTO, TbMovie>();

        // ShowTime mappings
        CreateMap<TbShowTime, ShowTimeResponseDTO>()
            .ForMember(dest => dest.MovieTitle, opt => opt.MapFrom(src => src.Movie.Title));
        CreateMap<ShowTimeCreateDTO, TbShowTime>();

        // Booking mappings
        CreateMap<TbBookingHistory, BookingResponseDTO>()
            .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => $"{src.User.FirstName} {src.User.LastName}"))
            .ForMember(dest => dest.MovieTitle, opt => opt.MapFrom(src => src.Movie.Title))
            .ForMember(dest => dest.Showtime, opt => opt.MapFrom(src => $"{src.Showtime.ShowDate} {src.Showtime.ShowTime}"));
        CreateMap<BookingCreateDTO, TbBookingHistory>();
    }
}