using FluentValidation;
using Pawesome.Models.ViewModels.Advert;

namespace Pawesome.Validators.Advert
{
    public class UpdatePetSittingRequestViewModelValidator : AbstractValidator<UpdatePetSittingRequestViewModel>
    {
        public UpdatePetSittingRequestViewModelValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty();
                
            RuleFor(x => x.StartDate)
                .NotEmpty().WithMessage("La date de début est obligatoire")
                .Must(date => date.Date >= DateTime.UtcNow.Date).WithMessage("La date de début doit être future");
                
            RuleFor(x => x.EndDate)
                .NotEmpty().WithMessage("La date de fin est obligatoire")
                .GreaterThan(x => x.StartDate).WithMessage("La date de fin doit être postérieure à la date de début");
                
            RuleFor(x => x.Amount)
                .GreaterThanOrEqualTo(0).WithMessage("Le montant ne peut pas être négatif");
                
            RuleFor(x => x.PetIds)
                .NotEmpty().WithMessage("Vous devez sélectionner au moins un animal");
                
            RuleFor(x => x.AdditionalInformation)
                .MaximumLength(500).WithMessage("Les informations supplémentaires ne peuvent pas dépasser 500 caractères");
        }
    }
}