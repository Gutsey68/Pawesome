
using System;
using System.ComponentModel.DataAnnotations;

namespace Pawesome.Models.Entities;

public class Report
{
    public int Id { get; set; }
    
    [StringLength(500, ErrorMessage = "Le commentaire ne doit pas dépasser 500 caractères")]
    public string? Comment { get; set; }
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    
    [Range(1, int.MaxValue, ErrorMessage = "L'identifiant de l'utilisateur doit être positif")]
    public int UserId { get; set; }
    
    public required User User { get; set; }
    
    public bool IsResolved { get; set; } = false;
    
    // Propriétés manquantes utilisées dans le contrôleur ReportMvcController
    [Range(1, int.MaxValue, ErrorMessage = "L'identifiant de la cible doit être positif")]
    public int TargetId { get; set; }
    
    [Required(ErrorMessage = "Le type de signalement est obligatoire")]
    [StringLength(50, ErrorMessage = "Le type de signalement ne doit pas dépasser 50 caractères")]
    public string ReportType { get; set; } = string.Empty;
    
    [StringLength(20, ErrorMessage = "Le statut ne doit pas dépasser 20 caractères")]
    public string Status { get; set; } = "Pending";
}