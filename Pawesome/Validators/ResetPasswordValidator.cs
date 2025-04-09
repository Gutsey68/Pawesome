using FluentValidation;
using Pawesome.Models.DTOs;

namespace Pawesome.Validators;

public class ResetPasswordValidator : AbstractValidator<ResetPasswordDto>
{
    public ResetPasswordValidator()
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