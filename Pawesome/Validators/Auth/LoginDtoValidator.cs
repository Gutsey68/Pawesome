using FluentValidation;
using Pawesome.Models.DTOs;
using Pawesome.Models.Dtos.Auth;

namespace Pawesome.Validators.Auth;

public class LoginDtoValidator : AbstractValidator<LoginDto>
{
    public LoginDtoValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("L'adresse e-mail est requise")
            .EmailAddress().WithMessage("Veuillez entrer une adresse e-mail valide");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Le mot de passe est requis");
    }
}
