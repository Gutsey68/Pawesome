using FluentValidation;
using Pawesome.Models.DTOs.Message;

namespace Pawesome.Validators.Message;

public class UpdateMessageDtoValidator : AbstractValidator<UpdateMessageDto>
{
    public UpdateMessageDtoValidator()
    {
        RuleFor(m => m.Status)
            .NotEmpty().WithMessage("Le statut du message est requis")
            .Must(status => status == "read" || status == "unread")
            .WithMessage("Le statut doit être 'read' ou 'unread'");
    }
}