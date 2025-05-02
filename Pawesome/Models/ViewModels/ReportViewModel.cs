using System.ComponentModel.DataAnnotations;

namespace Pawesome.Models.ViewModels;

public class CreateReportViewModel
{
    public int TargetId { get; set; }

    [Required(ErrorMessage = "Le type de signalement est obligatoire")]
    public required string ReportType { get; set; }

    [Required(ErrorMessage = "Le commentaire est obligatoire")]
    [StringLength(500, ErrorMessage = "Le commentaire ne doit pas dépasser 500 caractères")]
    [Display(Name = "Commentaire")]
    public string? Comment { get; set; }
}