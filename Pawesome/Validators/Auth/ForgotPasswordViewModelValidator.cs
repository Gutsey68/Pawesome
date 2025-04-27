using FluentValidation;
using Pawesome.Models.ViewModels;
using Pawesome.Models.ViewModels.Auth;

namespace Pawesome.Validators.Auth;

public class ForgotPasswordViewModelValidator : AbstractValidator<ForgotPasswordViewModel>
{
    public ForgotPasswordViewModelValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("L'email est requis")
            .EmailAddress().WithMessage("Format d'email invalide");
    }
}