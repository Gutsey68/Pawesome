using FluentValidation;
using Pawesome.Models.DTOs;

namespace Pawesome.Validators;

public class UpdatePetDtoValidator : AbstractValidator<UpdatePetDto>
{
    public UpdatePetDtoValidator()
    {
        RuleFor(p => p.Id)
            .NotEmpty().WithMessage("L'identifiant de l'animal est requis");
            
        RuleFor(p => p.Name)
            .NotEmpty().WithMessage("Le nom de l'animal est requis")
            .MaximumLength(255).WithMessage("Le nom ne peut pas dépasser 255 caractères");

        RuleFor(p => p.Breed)
            .MaximumLength(255).WithMessage("La race ne peut pas dépasser 255 caractères");

        RuleFor(p => p.Age)
            .InclusiveBetween(0, 30).When(p => p.Age.HasValue)
            .WithMessage("L'âge doit être compris entre 0 et 30 ans");

        RuleFor(p => p.AnimalTypeId)
            .NotEmpty().WithMessage("Le type d'animal est requis");

        RuleFor(p => p.Photo)
            .Must(photo => photo == null || 
                           (photo.Length <= 5 * 1024 * 1024 && 
                            (photo.ContentType == "image/jpeg" || 
                             photo.ContentType == "image/png" || 
                             photo.ContentType == "image/jpg")))
            .WithMessage("Le fichier doit être une image (jpg, jpeg, png) de moins de 5 Mo");
    }
}