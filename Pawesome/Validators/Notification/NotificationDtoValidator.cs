using FluentValidation;
using Pawesome.Models.Dtos.Notification;

namespace Pawesome.Validators.Notification;

public class NotificationDtoValidator : AbstractValidator<NotificationDto>
{
    public NotificationDtoValidator()
    {
        RuleFor(n => n.Title)
            .NotEmpty().WithMessage("Le titre est requis")
            .MaximumLength(100).WithMessage("Le titre ne peut pas dépasser 100 caractères");

        RuleFor(n => n.Message)
            .NotEmpty().WithMessage("Le message est requis")
            .MaximumLength(500).WithMessage("Le message ne peut pas dépasser 500 caractères");
        
        When(n => n.LinkUrl != null, () => {
            RuleFor(n => n.LinkUrl)
                .MaximumLength(250).WithMessage("L'URL ne peut pas dépasser 250 caractères");
        });
        
        When(n => n.ImageUrl != null, () => {
            RuleFor(n => n.ImageUrl)
                .MaximumLength(250).WithMessage("L'URL de l'image ne peut pas dépasser 250 caractères");
        });
    }
}