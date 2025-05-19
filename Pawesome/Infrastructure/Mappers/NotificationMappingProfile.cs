using AutoMapper;
using Pawesome.Models.Dtos.Notification;
using Pawesome.Models.Entities;

namespace Pawesome.Infrastructure.Mappers;

public class NotificationMappingProfile : Profile
{
    public NotificationMappingProfile()
    {
        CreateMap<Notification, NotificationDto>()
            .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.Type.ToString()));
    }
}