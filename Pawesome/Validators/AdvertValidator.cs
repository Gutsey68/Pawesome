using FluentValidation;
using Pawesome.Models;

namespace Pawesome.Validators;

public class AdvertValidator : AbstractValidator<Advert>
{
    public AdvertValidator()
    {
        RuleFor(a => a.StartDate)
            .NotEmpty().WithMessage("La date de début est requise")
            .LessThan(a => a.EndDate).WithMessage("La date de début doit être antérieure à la date de fin");
            
        RuleFor(a => a.EndDate)
            .NotEmpty().WithMessage("La date de fin est requise")
            .GreaterThan(a => a.StartDate).WithMessage("La date de fin doit être postérieure à la date de début");
            
        RuleFor(a => a.Status)
            .MaximumLength(255).WithMessage("Le statut ne peut pas dépasser 255 caractères")
            .Must(status => new[] { "pending", "active", "completed", "cancelled" }.Contains(status))
            .WithMessage("Le statut doit être 'pending', 'active', 'completed' ou 'cancelled'");
            
        RuleFor(a => a.Amount)
            .NotEmpty().WithMessage("Le montant est requis")
            .GreaterThan(0).WithMessage("Le montant doit être supérieur à 0");
    }
}
