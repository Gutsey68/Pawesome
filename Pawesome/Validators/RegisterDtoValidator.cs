using FluentValidation;
using Pawesome.Interfaces;
using Pawesome.Models.DTOs;

namespace Pawesome.Validators;

public class RegisterDtoValidator : AbstractValidator<RegisterDto>
{
    private readonly IUserRepository _userRepository;

    public RegisterDtoValidator(IUserRepository userRepository)
    {
        _userRepository = userRepository;

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("L'email est requis")
            .EmailAddress().WithMessage("Format d'email invalide")
            .MustAsync(BeUniqueEmail).WithMessage("Cet email est déjà utilisé");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Le mot de passe est requis")
            .MinimumLength(8).WithMessage("Le mot de passe doit contenir au moins 8 caractères")
            .Matches("[A-Z]").WithMessage("Le mot de passe doit contenir au moins une lettre majuscule")
            .Matches("[a-z]").WithMessage("Le mot de passe doit contenir au moins une lettre minuscule")
            .Matches("[0-9]").WithMessage("Le mot de passe doit contenir au moins un chiffre")
            .Matches("[^a-zA-Z0-9]").WithMessage("Le mot de passe doit contenir au moins un caractère spécial");

        RuleFor(x => x.ConfirmPassword)
            .NotEmpty().WithMessage("La confirmation du mot de passe est requise")
            .Equal(x => x.Password).WithMessage("Les mots de passe ne correspondent pas");

        RuleFor(x => x.FirstName)
            .NotEmpty().WithMessage("Le prénom est requis")
            .MaximumLength(50).WithMessage("Le prénom ne peut pas dépasser 50 caractères");

        RuleFor(x => x.LastName)
            .NotEmpty().WithMessage("Le nom est requis")
            .MaximumLength(50).WithMessage("Le nom ne peut pas dépasser 50 caractères");
    }

    private async Task<bool> BeUniqueEmail(string email, CancellationToken token)
    {
        return !await _userRepository.EmailExistsAsync(email);
    }
}