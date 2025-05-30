using AutoMapper;
using Pawesome.Models;
using Pawesome.Models.DTOs;
using Pawesome.Models.Entities;
using Pawesome.Models.ViewModels;
using Pawesome.Models.ViewModels.Pet;

namespace Pawesome.Infrastructure.Mappers;

public class PetMappingProfile : Profile
{
    public PetMappingProfile()
    {
        CreateMap<Pet, PetViewModel>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
            .ForMember(dest => dest.Species, opt => opt.MapFrom(src => src.AnimalType.Name))
            .ForMember(dest => dest.Photo, opt => opt.MapFrom(src => src.Photo));

        CreateMap<Pet, PetDetailsViewModel>()
            .ForMember(dest => dest.Species, opt => opt.MapFrom(src => src.AnimalType.Name))
            .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.UserId))
            .ForMember(dest => dest.OwnerName, opt => opt.MapFrom(src => $"{src.User.FirstName} {src.User.LastName}"));

        CreateMap<CreatePetViewModel, Pet>();

        CreateMap<Pet, UpdatePetViewModel>()
            .ForMember(dest => dest.ExistingPhoto, opt => opt.MapFrom(src => src.Photo))
            .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.UserId))
            .ForMember(dest => dest.Photo, opt => opt.Ignore());
    
        CreateMap<UpdatePetViewModel, Pet>()
            .ForMember(dest => dest.Photo, opt => opt.Ignore());

        CreateMap<Pet, PetSimpleDto>()
            .ForMember(dest => dest.AnimalTypeName, opt => opt.MapFrom(src => src.AnimalType.Name))
            .ForMember(dest => dest.Photo, opt => opt.MapFrom(src => src.Photo))
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id));
        
        CreateMap<PetSimpleDto, PetCartViewModel>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
            .ForMember(dest => dest.Species, opt => opt.MapFrom(src => src.AnimalTypeName))
            .ForMember(dest => dest.Photo, opt => opt.MapFrom(src => src.Photo))
            .ForMember(dest => dest.Info, opt => opt.MapFrom(src => src.Info));
    }
}