using AutoMapper;
using LibraryApi.DTOs;
using LibraryApi.Entities;

namespace LibraryApi.Utilities;

public class LibraryApiMappingProfile : Profile
{
    public LibraryApiMappingProfile()
    {
        CreateMap<SetReservationNotificationModel, ReservationNotification>()
            .ReverseMap();
        CreateMap<SetBookModel, Book>()
           .ReverseMap();
        CreateMap<GetBookModel, Book>()
           .ReverseMap();
    }
}
