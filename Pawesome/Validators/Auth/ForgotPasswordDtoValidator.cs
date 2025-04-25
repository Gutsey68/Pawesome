using FluentValidation;
using Pawesome.Models.DTOs;
using Pawesome.Models.Dtos.Auth;

namespace Pawesome.Validators.Auth;

public class ForgotPasswordDtoValidator : AbstractValidator<ForgotPasswordDto>
{
    public ForgotPasswordDtoValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("L'email est requis")
            .EmailAddress().WithMessage("Format d'email invalide");
    }
}