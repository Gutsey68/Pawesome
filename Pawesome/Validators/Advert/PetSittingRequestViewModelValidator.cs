using FluentValidation;
using Pawesome.Models.ViewModels.Advert;

namespace Pawesome.Validators.Advert;

public class PetSittingRequestViewModelValidator : AbstractValidator<PetSittingRequestViewModel>
{
    public PetSittingRequestViewModelValidator()
    {
        RuleFor(x => x.StartDate)
            .NotEmpty().WithMessage("La date de début est obligatoire")
            .Must(BeInFuture).WithMessage("La date de début doit être future");
            
        RuleFor(x => x.EndDate)
            .NotEmpty().WithMessage("La date de fin est obligatoire")
            .GreaterThan(x => x.StartDate).WithMessage("La date de fin doit être postérieure à la date de début");
            
        RuleFor(x => x.Amount)
            .GreaterThanOrEqualTo(1).WithMessage("Le montant doit être supérieur à 0");
            
        RuleFor(x => x.PetIds)
            .NotEmpty().WithMessage("Vous devez sélectionner au moins un animal");
    }
    
    private bool BeInFuture(DateTime date)
    {
        return date > DateTime.UtcNow.AddHours(-1);
    }
}