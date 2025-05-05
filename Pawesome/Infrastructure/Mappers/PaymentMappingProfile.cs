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
            .ForMember(dest => dest.IsPetSitter, opt => opt.MapFrom(src => 
                src.Advert.Status == "pending_offer"))
            .ForMember(dest => dest.StartDate, opt => opt.MapFrom(src => 
                src.Advert.StartDate))
            .ForMember(dest => dest.EndDate, opt => opt.MapFrom(src => 
                src.Advert.EndDate));
        
        CreateMap<PetSittingAdvertDto, CheckoutViewModel>()
            .ForMember(dest => dest.AdvertId, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.ServiceType, opt => opt.MapFrom(src => 
                src.IsPetSitter ? "Réservation de services de pet sitting" : "Réservation d'un pet sitter"))
            .ForMember(dest => dest.Description, opt => opt.MapFrom(src => 
                $"Du {src.StartDate:dd/MM/yyyy} au {src.EndDate:dd/MM/yyyy}"));
    }
}