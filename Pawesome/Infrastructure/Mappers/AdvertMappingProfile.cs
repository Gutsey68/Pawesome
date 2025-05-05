using AutoMapper;
using Microsoft.AspNetCore.Mvc.Rendering;
using Pawesome.Models;
using Pawesome.Models.DTOs;
using Pawesome.Models.Dtos.Advert;
using Pawesome.Models.Entities;
using Pawesome.Models.ViewModels;
using Pawesome.Models.ViewModels.Advert;

namespace Pawesome.Infrastructure.Mappers;

public class AdvertMappingProfile : Profile
{
    public AdvertMappingProfile()
    {
        CreateMap<Advert, PetSittingAdvertDto>()
            .ForMember(dest => dest.Owner, opt =>
                opt.MapFrom((src, _, _, context) => context.Mapper.Map<UserSimpleDto>(src.User)))
            .ForMember(dest => dest.PetCartViewModels, opt =>
                opt.MapFrom((src, _, _, context) =>
                {
                    return src.PetAdverts
                        .Where(pa => pa.Pet is { AnimalType: not null })
                        .Select(pa => context.Mapper.Map<PetSimpleDto>(pa.Pet))
                        .ToList();
                }))
            .ForMember(dest => dest.IsPetSitter, opt =>
                opt.MapFrom(src => src.Status.Contains("offer")))
            .ForMember(dest => dest.AdditionalInformation, opt => opt.MapFrom(src => src.AdditionalInformation));
        
        CreateMap<PetSittingRequestViewModel, Advert>()
            .ForMember(dest => dest.StartDate, opt => opt.MapFrom(src =>
                src.StartDate == default ? DateTime.UtcNow : DateTime.SpecifyKind(src.StartDate, DateTimeKind.Utc)))
            .ForMember(dest => dest.EndDate, opt => opt.MapFrom(src =>
                src.EndDate == default ? DateTime.UtcNow : DateTime.SpecifyKind(src.EndDate, DateTimeKind.Utc)))
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
            .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => DateTime.UtcNow));

        CreateMap<PetSittingOfferViewModel, Advert>()
            .ForMember(dest => dest.StartDate, opt => opt.MapFrom(src =>
                src.StartDate == default ? DateTime.UtcNow : DateTime.SpecifyKind(src.StartDate, DateTimeKind.Utc)))
            .ForMember(dest => dest.EndDate, opt => opt.MapFrom(src =>
                src.EndDate == default ? DateTime.UtcNow : DateTime.SpecifyKind(src.EndDate, DateTimeKind.Utc)))
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
            .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => DateTime.UtcNow));

        CreateMap<UpdatePetSittingRequestViewModel, Advert>()
            .ForMember(dest => dest.StartDate, opt => opt.MapFrom(src =>
                DateTime.SpecifyKind(src.StartDate, DateTimeKind.Utc)))
            .ForMember(dest => dest.EndDate, opt => opt.MapFrom(src =>
                DateTime.SpecifyKind(src.EndDate, DateTimeKind.Utc)))
            .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => DateTime.UtcNow));

        CreateMap<UpdatePetSittingOfferViewModel, Advert>()
            .ForMember(dest => dest.StartDate, opt => opt.MapFrom(src =>
                DateTime.SpecifyKind(src.StartDate, DateTimeKind.Utc)))
            .ForMember(dest => dest.EndDate, opt => opt.MapFrom(src =>
                DateTime.SpecifyKind(src.EndDate, DateTimeKind.Utc)))
            .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => DateTime.UtcNow));

        CreateMap<PetSimpleDto, PetCartViewModel>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
            .ForMember(dest => dest.Species, opt => opt.MapFrom(src => src.AnimalTypeName))
            .ForMember(dest => dest.Photo, opt => opt.MapFrom(src => src.Photo))
            .ForMember(dest => dest.Info, opt => opt.MapFrom(src => src.Info));
    }
    
}