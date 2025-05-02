using System.ComponentModel.DataAnnotations;

namespace Pawesome.Models.Entities;

public class Report
{
    public int Id { get; set; }
    
    public string? Comment { get; set; }
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    
    public int UserId { get; set; }
    
    public required User User { get; set; }
    
    public bool IsResolved { get; set; } = false;
    
    public int TargetId { get; set; }
    
    [MaxLength(50)]
    public string ReportType { get; set; } = string.Empty;
    
    [MaxLength(20)]
    public string Status { get; set; } = "Pending";
}