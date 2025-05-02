using Pawesome.Models.Entities;

namespace Pawesome.Interfaces;

public interface IReportService
{
    Task<IEnumerable<Report>> GetAllReportsAsync();
    Task<Report?> GetReportByIdAsync(int id);
    Task<Report> CreateReportAsync(Report report);
    Task<Report> UpdateReportAsync(Report report);
    Task<Report> ResolveReportAsync(int reportId);
}