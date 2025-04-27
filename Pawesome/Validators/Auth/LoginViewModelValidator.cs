using FluentValidation;
using Pawesome.Models.ViewModels.Auth;

namespace Pawesome.Validators.Auth;

public class LoginViewModelValidator : AbstractValidator<LoginViewModel>
{
    public LoginViewModelValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("L'adresse e-mail est requise")
            .EmailAddress().WithMessage("Veuillez entrer une adresse e-mail valide");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Le mot de passe est requis");
    }
}