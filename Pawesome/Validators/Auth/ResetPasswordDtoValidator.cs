using FluentValidation;
using Pawesome.Models.DTOs;
using Pawesome.Models.Dtos.Auth;

namespace Pawesome.Validators.Auth;

public class ResetPasswordDtoValidator : AbstractValidator<ResetPasswordDto>
{
    public ResetPasswordDtoValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("L'email est requis")
            .EmailAddress().WithMessage("Format d'email invalide");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Le mot de passe est requis")
            .MinimumLength(6).WithMessage("Le mot de passe doit contenir au moins 6 caractères");

        RuleFor(x => x.ConfirmPassword)
            .Equal(x => x.Password).WithMessage("Les mots de passe ne correspondent pas");

        RuleFor(x => x.Code)
            .NotEmpty().WithMessage("Le code de réinitialisation est invalide");
    }
}