using FluentValidation;
using Pawesome.Models;

namespace Pawesome.Validators;

public class PetValidator : AbstractValidator<Pet>
{
    private readonly AppDbContext _context;
    
    public PetValidator(AppDbContext context)
    {
        _context = context;
        
        RuleFor(p => p.Name)
            .NotEmpty().WithMessage("Le nom de l'animal est requis")
            .MaximumLength(255).WithMessage("Le nom ne peut pas dépasser 255 caractères");
            
        RuleFor(p => p.Breed)
            .MaximumLength(255).WithMessage("La race ne peut pas dépasser 255 caractères");
            
        RuleFor(p => p.Age)
            .GreaterThanOrEqualTo(0).WithMessage("L'âge doit être un nombre positif");
            
        RuleFor(p => p.Photo)
            .MaximumLength(255).WithMessage("L'URL de la photo ne peut pas dépasser 255 caractères");
            
        RuleFor(p => p.UserId)
            .NotEmpty().WithMessage("L'identifiant du propriétaire est requis")
            .MustAsync(async (userId, cancellationToken) => {
                return await _context.Users.FindAsync(new object[] { userId }, cancellationToken) != null;
            }).WithMessage("Le propriétaire spécifié n'existe pas");
            
        RuleFor(p => p.AnimalTypeId)
            .NotEmpty().WithMessage("Le type d'animal est requis")
            .MustAsync(async (typeId, cancellationToken) => {
                return await _context.AnimalTypes.FindAsync(new object[] { typeId }, cancellationToken) != null;
            }).WithMessage("Le type d'animal spécifié n'existe pas");
    }
}
