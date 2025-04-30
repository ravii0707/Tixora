using AutoMapper;
using Microsoft.Data.SqlClient;
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

        //Deactive Mappings
        CreateMap<TbMovie, MovieResponseDTO>();
        CreateMap<MovieCreateDTO, TbMovie>();
        // ShowTime mappings
        CreateMap<TbShowTime, ShowTimeResponseDTO>()
               .ForMember(dest => dest.MovieTitle, opt => opt.MapFrom(src => src.Movie.Title))
               .ForMember(dest => dest.ShowTime, opt => opt.MapFrom(src => src.ShowTime))
    // Remove the Split logic since ShowTime is a simple string
    .ForMember(dest => dest.ShowTime, opt => opt.MapFrom(src => src.ShowTime));


        CreateMap<ShowTimeCreateDTO, TbShowTime>();

        // Booking mappings
        CreateMap<TbBookingHistory, BookingResponseDTO>()
           .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => $"{src.User.FirstName}  {src.User.LastName}"))
            .ForMember(dest => dest.MovieTitle, opt => opt.MapFrom(src => src.Movie.Title))
            .ForMember(dest => dest.Showtime, opt => opt.MapFrom(src => $"{src.Showtime.ShowDate} {src.Showtime.ShowTime}"))
            .ForMember(dest => dest.TotalAmount, opt => opt.MapFrom(src => src.TotalAmount));
        CreateMap<BookingCreateDTO, TbBookingHistory>();
    }
}
