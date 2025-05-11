using AutoMapper;
using Pawesome.Models;
using Pawesome.Models.DTOs;
using Pawesome.Models.Entities;
using Pawesome.Models.ViewModels;
using Pawesome.Models.ViewModels.Auth;
using Pawesome.Models.ViewModels.User;

namespace Pawesome.Infrastructure.Mappers;

public class UserMappingProfile : Profile
{
    public UserMappingProfile()
    {
        CreateMap<RegisterViewModel, User>()
            .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.Email))
            .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
            .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.FirstName))
            .ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.LastName));

        CreateMap<User, ProfileViewModel>()
            .ForMember(dest => dest.Photo, opt => opt.MapFrom(src => src.Photo))
            .ForMember(dest => dest.Street,
                opt => opt.MapFrom(src => src.Address != null ? src.Address.StreetAddress : null))
            .ForMember(dest => dest.City, 
                opt => opt.MapFrom(src => src.Address != null ? src.Address.City.Name : null))
            .ForMember(dest => dest.PostalCode,
                opt => opt.MapFrom(src => src.Address != null ? src.Address.City.PostalCode : null))
            .ForMember(dest => dest.Country,
                opt => opt.MapFrom(src => src.Address != null ? 
                    src.Address.City.Country.Name : null))
            .ForMember(dest => dest.Pets, opt => opt.MapFrom(src => src.Pets));

        CreateMap<User, UserSimpleDto>()
            .ForMember(dest => dest.FullName, opt => opt.MapFrom(src =>
                $"{src.FirstName} {src.LastName}"));

        CreateMap<UpdateUserViewModel, User>()
            .ForMember(dest => dest.Photo, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow))
            .ForMember(dest => dest.Address, opt => opt.Ignore())
            .ForMember(dest => dest.Pets, opt => opt.Ignore())
            .ForMember(dest => dest.Notifications, opt => opt.Ignore())
            .ForMember(dest => dest.Reports, opt => opt.Ignore())
            .ForMember(dest => dest.PasswordResets, opt => opt.Ignore())
            .ForMember(dest => dest.SentMessages, opt => opt.Ignore())
            .ForMember(dest => dest.ReceivedMessages, opt => opt.Ignore())
            .ForMember(dest => dest.Reviews, opt => opt.Ignore())
            .ForMember(dest => dest.Payments, opt => opt.Ignore());

        CreateMap<User, UpdateUserViewModel>()
            .ForMember(dest => dest.ExistingPhoto, opt => opt.MapFrom(src => src.Photo))
            .ForMember(dest => dest.Photo, opt => opt.Ignore())
            .ForMember(dest => dest.StreetAddress, opt => opt.MapFrom(src => src.Address != null ? src.Address.StreetAddress : null))
            .ForMember(dest => dest.AdditionalInfo, opt => opt.MapFrom(src => src.Address != null ? src.Address.AdditionalInfo : null))
            .ForMember(dest => dest.City, opt => opt.MapFrom(src => src.Address != null ? src.Address.City.Name : null))
            .ForMember(dest => dest.PostalCode, opt => opt.MapFrom(src => src.Address != null ? src.Address.City.PostalCode : null))
            .ForMember(dest => dest.Country, opt => opt.MapFrom(src => src.Address != null ? 
                src.Address.City.Country.Name : null));
    }
}