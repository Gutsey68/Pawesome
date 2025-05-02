using FluentValidation;
using Pawesome.Models.ViewModels.Message;

namespace Pawesome.Validators.Message;

public class CreateMessageViewModelValidator : AbstractValidator<CreateMessageViewModel>
{
    public CreateMessageViewModelValidator()
    {
        RuleFor(m => m.Content)
            .NotEmpty().WithMessage("Le contenu du message est requis")
            .MaximumLength(1000).WithMessage("Le contenu du message ne doit pas dépasser 1000 caractères");

        RuleFor(m => m.ReceiverId)
            .NotEmpty().WithMessage("Le destinataire est requis");
    }
}