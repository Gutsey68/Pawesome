using System.ComponentModel.DataAnnotations;

namespace Pawesome.Models.ViewModels;

public class CreateReportViewModel
{
    public int TargetId { get; set; }
    public required string ReportType { get; set; }
    public required string Comment { get; set; }
}

