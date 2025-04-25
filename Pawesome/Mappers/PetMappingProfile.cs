using AutoMapper;
using Pawesome.Models;
using Pawesome.Models.DTOs;
using Pawesome.Models.Dtos.Pet;
using Pawesome.Models.ViewModels;

namespace Pawesome.Mappers;

public class PetMappingProfile : Profile
{
    public PetMappingProfile()
    {
        CreateMap<Pet, PetViewModel>()
            .ForMember(dest => dest.Species, opt => opt.MapFrom(src => src.AnimalType.Name));

        CreateMap<Pet, PetDetailsViewModel>()
            .ForMember(dest => dest.Species, opt => opt.MapFrom(src => src.AnimalType.Name))
            .ForMember(dest => dest.OwnerName, opt => opt.MapFrom(src => $"{src.User.FirstName} {src.User.LastName}"));

        CreateMap<CreatePetDto, Pet>();

        CreateMap<Pet, UpdatePetDto>()
            .ForMember(dest => dest.ExistingPhoto, opt => opt.MapFrom(src => src.Photo))
            .ForMember(dest => dest.Photo, opt => opt.Ignore());

        CreateMap<UpdatePetDto, Pet>()
            .ForMember(dest => dest.Photo, opt => opt.Ignore())
            .ForMember(dest => dest.User, opt => opt.Ignore())
            .ForMember(dest => dest.AnimalType, opt => opt.Ignore())
            .ForMember(dest => dest.PetAdverts, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow));
            
        CreateMap<Pet, PetSimpleDto>()
            .ForMember(dest => dest.AnimalTypeName, opt =>
                opt.MapFrom((src, _, _, context) => src.AnimalType != null ? src.AnimalType.Name : "Non spécifié"));
    }
}