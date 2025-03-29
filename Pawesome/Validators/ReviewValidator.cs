using FluentValidation;
using Pawesome.Models;

namespace Pawesome.Validators;

public class ReviewValidator : AbstractValidator<Review>
{
    private readonly AppDbContext _context;
    
    public ReviewValidator(AppDbContext context)
    {
        _context = context;
        
        RuleFor(r => r.Rate)
            .NotEmpty().WithMessage("La note est requise")
            .InclusiveBetween(1, 5).WithMessage("La note doit être comprise entre 1 et 5");
            
        RuleFor(r => r.UserId)
            .NotEmpty().WithMessage("L'identifiant de l'utilisateur est requis")
            .MustAsync(async (userId, cancellationToken) => {
                return await _context.Users.FindAsync(new object[] { userId }, cancellationToken) != null;
            }).WithMessage("L'utilisateur spécifié n'existe pas");
            
        RuleFor(r => r.AdvertId)
            .NotEmpty().WithMessage("L'identifiant de l'annonce est requis")
            .MustAsync(async (advertId, cancellationToken) => {
                return await _context.Adverts.FindAsync(new object[] { advertId }, cancellationToken) != null;
            }).WithMessage("L'annonce spécifiée n'existe pas");
    }
}
