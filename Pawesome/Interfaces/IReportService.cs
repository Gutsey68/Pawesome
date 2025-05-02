using Pawesome.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Pawesome.Interfaces;

public interface IReportService
{
    Task<IEnumerable<Report>> GetAllReportsAsync();
    Task<Report?> GetReportByIdAsync(int id);
    Task<Report> CreateReportAsync(Report report);
    Task<Report> UpdateReportAsync(Report report);
    Task<Report> ResolveReportAsync(int reportId);
}