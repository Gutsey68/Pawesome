using AutoMapper;
using Microsoft.AspNetCore.Mvc.Rendering;
using Pawesome.Models;
using Pawesome.Models.DTOs;
using Pawesome.Models.DTOs.Address;
using Pawesome.Models.Dtos.Advert;
using Pawesome.Models.Entities;
using Pawesome.Models.Enums;
using Pawesome.Models.ViewModels;
using Pawesome.Models.ViewModels.Advert;

namespace Pawesome.Infrastructure.Mappers;

public class AdvertMappingProfile : Profile
{
    public AdvertMappingProfile()
    {
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
            .ForMember(dest => dest.Info, opt => opt.MapFrom(src => src.Info))
            .ForMember(dest => dest.Species, opt => opt.MapFrom(src => src.AnimalTypeName));

        CreateMap<Advert, PetSittingAdvertDto>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.StartDate, opt => opt.MapFrom(src => src.StartDate))
            .ForMember(dest => dest.EndDate, opt => opt.MapFrom(src => src.EndDate))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()))
            .ForMember(dest => dest.Amount, opt => opt.MapFrom(src => src.Amount))
            .ForMember(dest => dest.AdditionalInformation, opt => opt.MapFrom(src => src.AdditionalInformation))
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt))
            .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => src.UpdatedAt))
            .ForMember(dest => dest.IsPetSitter, opt => opt.MapFrom(src => src.Status == AdvertStatus.PendingOffer))
            .ForMember(dest => dest.AnimalTypes, opt => opt.MapFrom(src => src.Status == AdvertStatus.PendingOffer 
                ? src.AnimalTypeAdverts.Select(ata => ata.AnimalType).ToList()
                : src.PetAdverts.Select(pa => pa.Pet!.AnimalType).ToList()))
            .ForMember(dest => dest.Owner, opt => opt.MapFrom(src => src.User))
            .ForMember(dest => dest.Pets, opt => opt.MapFrom(src => src.PetAdverts.Select(pa => pa.Pet).ToList()))
            .ForMember(dest => dest.Address, opt => opt.MapFrom(src => src.User.Address))
            .ForMember(dest => dest.City, opt => opt.MapFrom(src => src.User.Address != null ? src.User.Address.City.Name : null));
        
        CreateMap<Address, AddressDto>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.StreetAddress, opt => opt.MapFrom(src => src.StreetAddress))
            .ForMember(dest => dest.CityId, opt => opt.MapFrom(src => src.CityId))
            .ForMember(dest => dest.CityName, opt => opt.MapFrom(src => src.City.Name))
            .ForMember(dest => dest.PostalCode, opt => opt.MapFrom(src => src.City.PostalCode))
            .ForMember(dest => dest.CountryId, opt => opt.MapFrom(src => src.City.CountryId))
            .ForMember(dest => dest.CountryName, opt => opt.MapFrom(src => src.City.Country.Name));
        
        CreateMap<Advert, AdvertDto>()
            .ForMember(dest => dest.OwnerName, opt => opt.MapFrom(src => $"{src.User.FirstName} {src.User.LastName}"))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()))
            .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.Status == AdvertStatus.PendingOffer ? "Offre" : "Demande"))
            .ForMember(dest => dest.City, opt => opt.MapFrom(src => 
                src.User.Address != null ? src.User.Address.City.Name : "Non spécifiée"));
        
    }
}