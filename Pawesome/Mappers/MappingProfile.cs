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
                opt => opt.MapFrom(src => src.Address != null ? src.Address.StreetAddress : null))
            .ForMember(dest => dest.City, opt => opt.MapFrom(src => src.Address != null ? src.Address.City.Name : null))
            .ForMember(dest => dest.PostalCode,
                opt => opt.MapFrom(src => src.Address != null ? src.Address.City.PostalCode : null))
            .ForMember(dest => dest.Country,
                opt => opt.MapFrom(src => src.Address != null ? src.Address.City.Country.Name : null))
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
    }
}