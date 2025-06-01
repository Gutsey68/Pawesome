using FluentValidation;
using Pawesome.Models.ViewModels;

namespace Pawesome.Validators.Report;

public class CreateReportViewModelValidator : AbstractValidator<CreateReportViewModel>
{
    public CreateReportViewModelValidator()
    {
        RuleFor(r => r.TargetId)
            .GreaterThan(0).WithMessage("L'identifiant de la cible est requis.");

        RuleFor(r => r.ReportType)
            .NotEmpty().WithMessage("Le type de signalement est requis.")
            .MaximumLength(100).WithMessage("Le type de signalement ne peut pas dépasser 100 caractères.");

        RuleFor(r => r.Comment)
            .NotEmpty().WithMessage("Le commentaire est requis.")
            .MaximumLength(1000).WithMessage("Le commentaire ne peut pas dépasser 1000 caractères.");
    }
    
}

