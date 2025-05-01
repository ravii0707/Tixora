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

        CreateMap<MovieWithShowTimesDTO, TbMovie>()
           .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Movie.Title))
           .ForMember(dest => dest.Genre, opt => opt.MapFrom(src => src.Movie.Genre))
           // Map all other properties explicitly
           .ForMember(dest => dest.Language, opt => opt.MapFrom(src => src.Movie.Language))
           .ForMember(dest => dest.Format, opt => opt.MapFrom(src => src.Movie.Format))
           .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Movie.Description))
           .ForMember(dest => dest.ImageUrl, opt => opt.MapFrom(src => src.Movie.ImageUrl));

        CreateMap<MovieCreateDTO, TbMovie>()
        .ForMember(dest => dest.MovieId, opt => opt.Ignore())
    .ForMember(dest => dest.TbBookingHistories, opt => opt.Ignore())
    .ForMember(dest => dest.TbShowTimes, opt => opt.Ignore());

        // ShowTime mappings
        CreateMap<TbShowTime, ShowTimeResponseDTO>()
         .ForMember(dest => dest.MovieTitle, opt => opt.MapFrom(src => src.Movie.Title))
         .ForMember(dest => dest.Genre, opt => opt.MapFrom(src => src.Movie.Genre))
         .ForMember(dest => dest.Language, opt => opt.MapFrom(src => src.Movie.Language))
         .ForMember(dest => dest.Format, opt => opt.MapFrom(src => src.Movie.Format))
         .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Movie.Description))
         .ForMember(dest => dest.ImageUrl, opt => opt.MapFrom(src => src.Movie.ImageUrl))
         .ForMember(dest => dest.IsMovieActive, opt => opt.MapFrom(src => src.Movie.IsActive))
         .ForMember(dest => dest.ShowTime, opt => opt.MapFrom(src => src.ShowTime));

        //CreateMap<TbShowTime, ShowTimeCreateDTO>() 
        //    .IncludeMembers(src => src.Movie) 
        //    ForMember(dest => dest.ShowTime, opt => opt.MapFrom(src => src.ShowTime));

        CreateMap<ShowTimeCreateDTO, TbShowTime>();
        CreateMap<ShowTimeUpdateDTO, TbShowTime>();


        // Booking mappings
        CreateMap<TbBookingHistory, BookingResponseDTO>()
          .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => $"{src.User.FirstName} {src.User.LastName}"))
    .ForMember(dest => dest.MovieTitle, opt => opt.MapFrom(src => src.Movie.Title))
    .ForMember(dest => dest.ShowTime, opt => opt.MapFrom(src => src.Showtime.ShowTime))
    .ForMember(dest => dest.ShowDate, opt => opt.MapFrom(src => src.Showtime.ShowDate))
    .ForMember(dest => dest.TotalAmount, opt => opt.MapFrom(src => src.TotalAmount));
        CreateMap<BookingCreateDTO, TbBookingHistory>();

        //moviewithshowtime
    

        //// Booking mappings
        //CreateMap<TbBookingHistory, BookingResponseDTO>()
        //        .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => $"{src.User.FirstName} {src.User.LastName}"))
        //        .ForMember(dest => dest.MovieTitle, opt => opt.MapFrom(src => src.Movie.Title))

        //        .ForMember(dest => dest.TotalAmount, opt => opt.MapFrom(src => src.TotalAmount));
        //CreateMap<BookingCreateDTO, TbBookingHistory>();
    }
}
