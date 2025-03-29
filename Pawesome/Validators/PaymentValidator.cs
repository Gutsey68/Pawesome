using FluentValidation;
using Pawesome.Models;

namespace Pawesome.Validators;

public class PaymentValidator : AbstractValidator<Payment>
{
    private readonly AppDbContext _context;
    
    public PaymentValidator(AppDbContext context)
    {
        _context = context;
        
        RuleFor(p => p.Amount)
            .NotEmpty().WithMessage("Le montant est requis")
            .GreaterThan(0).WithMessage("Le montant doit être supérieur à 0");
            
        RuleFor(p => p.Status)
            .MaximumLength(255).WithMessage("Le statut ne peut pas dépasser 255 caractères")
            .Must(status => new[] { "pending", "completed", "failed", "refunded" }.Contains(status))
            .WithMessage("Le statut doit être 'pending', 'completed', 'failed' ou 'refunded'");
            
        RuleFor(p => p.UserId)
            .NotEmpty().WithMessage("L'identifiant de l'utilisateur est requis")
            .MustAsync(async (userId, cancellationToken) => {
                return await _context.Users.FindAsync(new object[] { userId }, cancellationToken) != null;
            }).WithMessage("L'utilisateur spécifié n'existe pas");
            
        RuleFor(p => p.AdvertId)
            .NotEmpty().WithMessage("L'identifiant de l'annonce est requis")
            .MustAsync(async (advertId, cancellationToken) => {
                return await _context.Adverts.FindAsync(new object[] { advertId }, cancellationToken) != null;
            }).WithMessage("L'annonce spécifiée n'existe pas");
    }
}
