using FluentValidation;
using Pawesome.Models;

namespace Pawesome.Validators;

public class AddressValidator : AbstractValidator<Address>
{
    private readonly AppDbContext _context;
    
    public AddressValidator(AppDbContext context)
    {
        _context = context;
        
        RuleFor(a => a.StreetAddress)
            .NotEmpty().WithMessage("L'adresse est requise")
            .MaximumLength(255).WithMessage("L'adresse ne peut pas dépasser 255 caractères");
            
        RuleFor(a => a.AdditionalInfo)
            .MaximumLength(255).WithMessage("Le complément d'adresse ne peut pas dépasser 255 caractères");
            
        RuleFor(a => a.CityId)
            .NotEmpty().WithMessage("La ville est requise")
            .MustAsync(async (cityId, cancellationToken) => {
                return await _context.Cities.FindAsync(new object[] { cityId }, cancellationToken) != null;
            }).WithMessage("La ville spécifiée n'existe pas");
    }
}
