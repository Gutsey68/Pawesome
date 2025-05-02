using AutoMapper;
using Pawesome.Models.DTOs.Message;
using Pawesome.Models.Entities;
using Pawesome.Models.ViewModels.Message;

namespace Pawesome.Infrastructure.Mappers;

public class MessageMappingProfile : Profile
{
    public MessageMappingProfile()
    {
        CreateMap<Message, MessageDto>()
            .ForMember(dest => dest.SenderFullName,
                opt => opt.MapFrom(src => $"{src.Sender.FirstName} {src.Sender.LastName}"))
            .ForMember(dest => dest.ReceiverFullName,
                opt => opt.MapFrom(src => $"{src.Receiver.FirstName} {src.Receiver.LastName}"))
            .ForMember(dest => dest.SenderPhoto,
                opt => opt.MapFrom(src => src.Sender.Photo));
        
        CreateMap<MessageDto, MessageViewModel>()
            .ForMember(dest => dest.FormattedDate,
                opt => opt.MapFrom(src => src.CreatedAt.ToString("dd/MM/yyyy HH:mm")));
        
        CreateMap<CreateMessageViewModel, CreateMessageDto>();
    }
}