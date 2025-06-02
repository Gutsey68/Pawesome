using AutoMapper;
using Pawesome.Models.Dtos.Booking;
using Pawesome.Models.DTOs.Booking;
using Pawesome.Models.Entities;

namespace Pawesome.Infrastructure.Mappers
{
    public class BookingMappingProfile : Profile
    {
        public BookingMappingProfile()
        {
            CreateMap<Booking, BookingDto>()
                .ForMember(dest => dest.AdvertTitle, opt => opt.MapFrom(src =>
                    src.Advert.AdditionalInformation ?? "Annonce"))
                .ForMember(dest => dest.BookerUserName, opt => opt.MapFrom(src =>
                    $"{src.BookerUser.FirstName} {src.BookerUser.LastName}"))
                .ForMember(dest => dest.BookerUserEmail, opt => opt.MapFrom(src =>
                    src.BookerUser.Email))
                .ForMember(dest => dest.PetSitterUserId, opt => opt.MapFrom(src =>
                    src.Advert.UserId))
                .ForMember(dest => dest.PetSitterUserName, opt => opt.MapFrom(src =>
                    $"{src.Advert.User.FirstName} {src.Advert.User.LastName}"))
                .ForMember(dest => dest.PetSitterPhoto, opt => opt.MapFrom(src =>
                    src.Advert.User.Photo))
                .ForMember(dest => dest.AdditionalInformation,
                    opt => opt.MapFrom(src => src.Advert.AdditionalInformation))
                .ForMember(dest => dest.BookerPhoto, opt => opt.MapFrom(src =>
                    src.BookerUser.Photo)).ForMember(dest => dest.Pets, opt => opt.MapFrom(src =>
                    src.Advert.PetAdverts.Select(ap => ap.Pet)))
                .ForMember(dest => dest.AdvertStatus, opt => opt.MapFrom(src => src.Advert.Status));

                
            CreateMap<BookingDto, Booking>();
            
            CreateMap<UpdateBookingDto, Booking>()
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => DateTime.UtcNow));
                
            CreateMap<CreateBookingDto, Booking>()
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => 
                    Pawesome.Models.enums.BookingStatus.PendingConfirmation));
        }
    }
}