using AutoMapper;
using Pawesome.Models;
using Pawesome.Models.DTOs;
using Pawesome.Models.Entities;
using Pawesome.Models.ViewModels;

namespace Pawesome.Infrastructure.Mappers;

public class AnimalTypeMappingProfile : Profile
{
    public AnimalTypeMappingProfile()
    {
        CreateMap<AnimalType, AnimalTypeViewModel>();
        CreateMap<AnimalType, AnimalTypeDto>();
    }
}