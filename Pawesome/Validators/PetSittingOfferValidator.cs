using FluentValidation;
using Pawesome.Models.DTOs;

namespace Pawesome.Validators;

public class PetSittingOfferValidator : AbstractValidator<PetSittingOfferDto>
{
    public PetSittingOfferValidator()
    {
        RuleFor(x => x.StartDate)
            .NotEmpty().WithMessage("La date de début est obligatoire")
            .Must(BeInFuture).WithMessage("La date de début doit être future");
            
        RuleFor(x => x.EndDate)
            .NotEmpty().WithMessage("La date de fin est obligatoire")
            .GreaterThan(x => x.StartDate).WithMessage("La date de fin doit être postérieure à la date de début");
            
        RuleFor(x => x.Amount)
            .GreaterThanOrEqualTo(0).WithMessage("Le montant ne peut pas être négatif");
            
        RuleFor(x => x.AcceptedAnimalTypeIds)
            .NotEmpty().WithMessage("Vous devez sélectionner au moins un type d'animal");
    }
    
    private bool BeInFuture(DateTime date)
    {
        return date > DateTime.UtcNow.AddHours(-1);
    }
}