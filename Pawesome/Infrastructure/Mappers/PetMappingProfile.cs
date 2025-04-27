using AutoMapper;
using Pawesome.Models;
using Pawesome.Models.DTOs;
using Pawesome.Models.ViewModels;
using Pawesome.Models.ViewModels.Pet;

namespace Pawesome.Infrastructure.Mappers;

public class PetMappingProfile : Profile
{
    public PetMappingProfile()
    {
        CreateMap<Pet, PetViewModel>()
            .ForMember(dest => dest.Species, opt => opt.MapFrom(src => src.AnimalType.Name));

        CreateMap<Pet, PetDetailsViewModel>()
            .ForMember(dest => dest.Species, opt => opt.MapFrom(src => src.AnimalType.Name))
            .ForMember(dest => dest.OwnerName, opt => opt.MapFrom(src => $"{src.User.FirstName} {src.User.LastName}"));

        CreateMap<CreatePetViewModel, Pet>();

        CreateMap<Pet, UpdatePetViewModel>()
            .ForMember(dest => dest.ExistingPhoto, opt => opt.MapFrom(src => src.Photo))
            .ForMember(dest => dest.Photo, opt => opt.Ignore());
    
        CreateMap<UpdatePetViewModel, Pet>()
            .ForMember(dest => dest.Photo, opt => opt.Ignore());
            
        CreateMap<Pet, PetSimpleDto>()
            .ForMember(dest => dest.AnimalTypeName, opt =>
                opt.MapFrom((src, _, _, context) => src.AnimalType != null ? src.AnimalType.Name : "Non spécifié"));
    }
}