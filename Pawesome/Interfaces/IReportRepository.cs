using Pawesome.Models.Entities;

namespace Pawesome.Interfaces;

public interface IReportRepository
{
    Task<IEnumerable<Report>> GetAllReportsAsync();
    Task<Report?> GetReportByIdAsync(int id);
    Task AddReportAsync(Report report);
    Task UpdateReportAsync(Report report);
    Task<int> GetReportsCountAsync();
}