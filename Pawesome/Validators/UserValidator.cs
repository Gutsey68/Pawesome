using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Pawesome.Models;

namespace Pawesome.Validators;

public class UserValidator : AbstractValidator<User>
{
    private readonly AppDbContext _context;

    public UserValidator(AppDbContext context)
    {
        _context = context;
            
        RuleFor(u => u.FirstName)
            .NotEmpty().WithMessage("Le prénom est requis")
            .MaximumLength(255).WithMessage("Le prénom ne peut pas dépasser 255 caractères");
                
        RuleFor(u => u.LastName)
            .NotEmpty().WithMessage("Le nom est requis")
            .MaximumLength(255).WithMessage("Le nom ne peut pas dépasser 255 caractères");
                
        RuleFor(u => u.Email)
            .NotEmpty().WithMessage("L'email est requis")
            .EmailAddress().WithMessage("Format d'email invalide")
            .MaximumLength(255).WithMessage("L'email ne peut pas dépasser 255 caractères")
            .MustAsync(BeUniqueEmail).WithMessage("Cet email est déjà utilisé");
                
        RuleFor(u => u.Password)
            .NotEmpty().WithMessage("Le mot de passe est requis")
            .MinimumLength(8).WithMessage("Le mot de passe doit contenir au moins 8 caractères")
            .MaximumLength(255).WithMessage("Le mot de passe ne peut pas dépasser 255 caractères");
        
        RuleFor(u => u.Bio)
            .MaximumLength(1000).WithMessage("La biographie ne peut pas dépasser 1000 caractères");
            
        RuleFor(u => u.Photo)
            .MaximumLength(255).WithMessage("L'URL de la photo ne peut pas dépasser 255 caractères");
            
        RuleFor(u => u.Rating)
            .InclusiveBetween(0, 5).WithMessage("La note doit être comprise entre 0 et 5");
            
        RuleFor(u => u.BalanceAccount)
            .GreaterThanOrEqualTo(0).WithMessage("Le solde ne peut pas être négatif");
            
        RuleFor(u => u.OnboardingStep)
            .GreaterThanOrEqualTo(1).WithMessage("L'étape d'onboarding doit être au moins 1");
            
        RuleFor(u => u.CompletedProfile)
            .InclusiveBetween(0, 100).WithMessage("Le pourcentage de complétion du profil doit être entre 0 et 100");
                
        When(u => u.AddressId.HasValue, () => {
            RuleFor(u => u.Address).NotNull().WithMessage("L'adresse associée est invalide");
        });
            
        When(u => u.RoleId > 0, () => {
            RuleFor(u => u.Role).NotNull().WithMessage("Le rôle associé est invalide");
        });
    }
        
    private async Task<bool> BeUniqueEmail(string email, CancellationToken cancellationToken)
    {
        return !await _context.Users
            .AnyAsync(u => u.Email == email, cancellationToken);
    }
}