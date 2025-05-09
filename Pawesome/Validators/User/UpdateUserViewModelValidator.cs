using FluentValidation;
using Pawesome.Models.ViewModels.User;

namespace Pawesome.Validators.User;

public class UpdateUserViewModelValidator: AbstractValidator<UpdateUserViewModel>
{
    public UpdateUserViewModelValidator()
    {
        RuleFor(u => u.FirstName)
            .NotEmpty().WithMessage("Le prénom est requis")
            .MaximumLength(255).WithMessage("Le prénom ne peut pas dépasser 255 caractères");

        RuleFor(u => u.LastName)
            .NotEmpty().WithMessage("Le nom est requis")
            .MaximumLength(255).WithMessage("Le nom ne peut pas dépasser 255 caractères");

        RuleFor(u => u.Bio)
            .MaximumLength(1000).When(u => !string.IsNullOrEmpty(u.Bio))
            .WithMessage("La biographie ne peut pas dépasser 1000 caractères");

        RuleFor(u => u.PhoneNumber)
            .Matches(@"^(?:(?:\+|00)33|0)\s*[1-9](?:[\s.-]*\d{2}){4}$")
            .When(u => !string.IsNullOrEmpty(u.PhoneNumber))
            .WithMessage("Le numéro de téléphone doit être au format français (ex: 06 12 34 56 78)");

        RuleFor(u => u.Photo)
            .Must(photo => photo == null ||
                           (photo.Length <= 5 * 1024 * 1024 &&
                            (photo.ContentType == "image/jpeg" ||
                             photo.ContentType == "image/png" ||
                             photo.ContentType == "image/jpg")))
            .WithMessage("Le fichier doit être une image (jpg, jpeg, png) de moins de 5 Mo");
    }
}