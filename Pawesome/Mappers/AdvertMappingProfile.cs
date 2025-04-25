using AutoMapper;
using Pawesome.Models;
using Pawesome.Models.DTOs;
using Pawesome.Models.Dtos.Advert;

namespace Pawesome.Mappers;

public class AdvertMappingProfile : Profile
{
    public AdvertMappingProfile()
    {
        CreateMap<Advert, PetSittingAdvertDto>()
            .ForMember(dest => dest.Owner, opt =>
                opt.MapFrom((src, _, _, context) => {
                    if (src.PetAdverts.Count == 0)
                        return null;

                    var petAdvert = src.PetAdverts.FirstOrDefault();
                    if (petAdvert == null || petAdvert.Pet == null)
                        return null;

                    return context.Mapper.Map<UserSimpleDto>(petAdvert.Pet.User);
                }))
            .ForMember(dest => dest.Pets, opt =>
                opt.MapFrom((src, _, _, context) => {
                    return src.PetAdverts
                        .Where(pa => pa.Pet != null && pa.Pet.AnimalType != null)
                        .Select(pa => context.Mapper.Map<PetSimpleDto>(pa.Pet))
                        .ToList();
                }))
            .ForMember(dest => dest.IsPetSitter, opt =>
                opt.MapFrom(src => src.Status.Contains("offer")));

        CreateMap<PetSittingRequestDto, Advert>()
            .ForMember(dest => dest.StartDate, opt => opt.MapFrom(src =>
                src.StartDate == default ? DateTime.UtcNow : DateTime.SpecifyKind(src.StartDate, DateTimeKind.Utc)))
            .ForMember(dest => dest.EndDate, opt => opt.MapFrom(src =>
                src.EndDate == default ? DateTime.UtcNow : DateTime.SpecifyKind(src.EndDate, DateTimeKind.Utc)))
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
            .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => DateTime.UtcNow));

        CreateMap<PetSittingOfferDto, Advert>()
            .ForMember(dest => dest.StartDate, opt => opt.MapFrom(src =>
                src.StartDate == default ? DateTime.UtcNow : DateTime.SpecifyKind(src.StartDate, DateTimeKind.Utc)))
            .ForMember(dest => dest.EndDate, opt => opt.MapFrom(src =>
                src.EndDate == default ? DateTime.UtcNow : DateTime.SpecifyKind(src.EndDate, DateTimeKind.Utc)))
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
            .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => DateTime.UtcNow));
    }
}