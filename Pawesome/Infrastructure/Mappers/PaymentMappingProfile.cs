using AutoMapper;
using Pawesome.Models.Dtos.Advert;
using Pawesome.Models.Dtos.Payment;
using Pawesome.Models.Entities;
using Pawesome.Models.ViewModels.Payment;

namespace Pawesome.Infrastructure.Mappers;

public class PaymentMappingProfile : Profile
{
    public PaymentMappingProfile()
    {
        CreateMap<Payment, PaymentDto>();

        CreateMap<Payment, PaymentHistoryViewModel>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.Amount, opt => opt.MapFrom(src => src.Amount))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()))
            .ForMember(dest => dest.SessionId, opt => opt.MapFrom(src => src.SessionId))
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt))
            .ForMember(dest => dest.AdvertId, opt => opt.MapFrom(src => src.Booking.AdvertId))
            .ForMember(dest => dest.IsPetSitter, opt => opt.MapFrom(src => src.Booking.Advert.Status == Models.Enums.AdvertStatus.PendingOffer))
            .ForMember(dest => dest.StartDate, opt => opt.MapFrom(src => src.Booking.StartDate))
            .ForMember(dest => dest.EndDate, opt => opt.MapFrom(src => src.Booking.EndDate));
        
        CreateMap<PetSittingAdvertDto, CheckoutViewModel>()
            .ForMember(dest => dest.AdvertId, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.ServiceType, opt => opt.MapFrom(src => 
                src.IsPetSitter ? "Réservation de services de pet sitting" : "Réservation d'un pet sitter"))
            .ForMember(dest => dest.Description, opt => opt.MapFrom(src => 
                $"Du {src.StartDate:dd/MM/yyyy} au {src.EndDate:dd/MM/yyyy}"));
    }
}