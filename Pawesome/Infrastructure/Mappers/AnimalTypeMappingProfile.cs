using AutoMapper;
using Pawesome.Models;
using Pawesome.Models.DTOs;
using Pawesome.Models.Dtos.AnimalType;
using Pawesome.Models.Entities;
using Pawesome.Models.ViewModels;
using Pawesome.Models.ViewModels.AnimalType;

namespace Pawesome.Infrastructure.Mappers;

public class AnimalTypeMappingProfile : Profile
{
    public AnimalTypeMappingProfile()
    {
        CreateMap<AnimalType, AnimalTypeDto>();
        
        CreateMap<AnimalType, AnimalTypeViewModel>()
            .ForMember(dest => dest.AnimalType, opt => opt.MapFrom(src => src))
            .ForMember(dest => dest.IsSelected, opt => opt.MapFrom(src => false));
    }
}
