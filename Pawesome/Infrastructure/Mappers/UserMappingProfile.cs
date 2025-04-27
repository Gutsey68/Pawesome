using AutoMapper;
using Pawesome.Models;
using Pawesome.Models.DTOs;
using Pawesome.Models.ViewModels;
using Pawesome.Models.ViewModels.Auth;

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
            .ForMember(dest => dest.Street,
                opt => opt.MapFrom(src => src.Address.StreetAddress))
            .ForMember(dest => dest.City, opt => opt.MapFrom(src => src.Address.City.Name))
            .ForMember(dest => dest.PostalCode,
                opt => opt.MapFrom(src => src.Address.City.PostalCode))
            .ForMember(dest => dest.Country,
                opt => opt.MapFrom(src => src.Address.City.Country.Name))
            .ForMember(dest => dest.Pets, opt => opt.MapFrom(src => src.Pets));
            
        CreateMap<User, UserSimpleDto>()
            .ForMember(dest => dest.FullName, opt => opt.MapFrom(src =>
                $"{src.FirstName} {src.LastName}"));
    }
}