using AutoMapper;
using Pawesome.Models;
using Pawesome.Models.DTOs;
using Pawesome.Models.ViewModels;

namespace Pawesome.Mappers;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<RegisterDto, User>()
            .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.Email))
            .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
            .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.FirstName))
            .ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.LastName));

        CreateMap<User, ProfileViewModel>()
            .ForMember(dest => dest.Street,
                opt => opt.MapFrom(src => src.Address.StreetAddress))
            .ForMember(dest => dest.City, opt => opt.MapFrom(src => src.Address.City.Name))
            .ForMember(dest => dest.PostalCode,
                opt => opt.MapFrom(src => src.Address.City.PostalCode))
            .ForMember(dest => dest.Country,
                opt => opt.MapFrom(src => src.Address.City.Country.Name))
            .ForMember(dest => dest.Pets, opt => opt.MapFrom(src => src.Pets));

        CreateMap<Pet, PetViewModel>()
            .ForMember(dest => dest.Species, opt => opt.MapFrom(src => src.AnimalType.Name));

        CreateMap<Pet, PetDetailsViewModel>()
            .ForMember(dest => dest.Species, opt => opt.MapFrom(src => src.AnimalType.Name))
            .ForMember(dest => dest.OwnerName, opt => opt.MapFrom(src => $"{src.User.FirstName} {src.User.LastName}"));

        CreateMap<CreatePetDto, Pet>();

        CreateMap<AnimalType, AnimalTypeViewModel>();

        CreateMap<Pet, UpdatePetDto>()
            .ForMember(dest => dest.ExistingPhoto, opt => opt.MapFrom(src => src.Photo))
            .ForMember(dest => dest.Photo, opt => opt.Ignore());

        CreateMap<UpdatePetDto, Pet>()
            .ForMember(dest => dest.Photo, opt => opt.Ignore())
            .ForMember(dest => dest.User, opt => opt.Ignore())
            .ForMember(dest => dest.AnimalType, opt => opt.Ignore())
            .ForMember(dest => dest.PetAdverts, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow));

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

        CreateMap<User, UserSimpleDto>()
            .ForMember(dest => dest.FullName, opt => opt.MapFrom(src =>
                $"{src.FirstName} {src.LastName}"));

        CreateMap<Pet, PetSimpleDto>()
            .ForMember(dest => dest.AnimalTypeName, opt =>
                opt.MapFrom((src, _, _, context) => src.AnimalType != null ? src.AnimalType.Name : "Non spécifié"));

        CreateMap<AnimalType, AnimalTypeDto>();

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