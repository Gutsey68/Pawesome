using Pawesome.Interfaces;
using Pawesome.Models.Entities;

namespace Pawesome.Services;

/// <summary>
/// Service responsible for handling report-related operations.
/// </summary>
public class ReportService : IReportService
{
    private readonly IReportRepository _reportRepository;

    /// <summary>
    /// Initializes a new instance of the <see cref="ReportService"/> class.
    /// </summary>
    /// <param name="reportRepository">The report repository.</param>
    public ReportService(IReportRepository reportRepository)
    {
        _reportRepository = reportRepository;
    }

    /// <summary>
    /// Gets all reports asynchronously.
    /// </summary>
    /// <returns>A collection of all reports.</returns>
    public async Task<IEnumerable<Report>> GetAllReportsAsync()
    {
        return await _reportRepository.GetAllReportsAsync();
    }

    /// <summary>
    /// Gets a report by its identifier asynchronously.
    /// </summary>
    /// <param name="id">The report identifier.</param>
    /// <returns>The report with the specified identifier, or null if not found.</returns>
    /// <exception cref="ArgumentException">Thrown when the identifier is not positive.</exception>
    public async Task<Report?> GetReportByIdAsync(int id)
    {
        if (id <= 0)
            throw new ArgumentException("L'identifiant du signalement doit être positif.", nameof(id));

        return await _reportRepository.GetReportByIdAsync(id);
    }

    /// <summary>
    /// Creates a new report asynchronously.
    /// </summary>
    /// <param name="report">The report to create.</param>
    /// <returns>The created report.</returns>
    /// <exception cref="ArgumentNullException">Thrown when the report is null.</exception>
    /// <exception cref="ArgumentException">Thrown when the report data is invalid.</exception>
    public async Task<Report> CreateReportAsync(Report report)
    {
        ValidateReport(report);
        
        report.CreatedAt = DateTime.UtcNow;
        report.UpdatedAt = DateTime.UtcNow;
        
        await _reportRepository.AddReportAsync(report);
        return report;
    }

    /// <summary>
    /// Updates an existing report asynchronously.
    /// </summary>
    /// <param name="report">The report to update.</param>
    /// <returns>The updated report.</returns>
    /// <exception cref="ArgumentException">Thrown when the report identifier is not positive or report data is invalid.</exception>
    /// <exception cref="ArgumentNullException">Thrown when the report is null.</exception>
    public async Task<Report> UpdateReportAsync(Report report)
    {
        if (report.Id <= 0)
            throw new ArgumentException("L'identifiant du signalement doit être positif.", nameof(report.Id));
            
        ValidateReport(report);
        
        report.UpdatedAt = DateTime.UtcNow;
        
        await _reportRepository.UpdateReportAsync(report);
        return report;
    }
    
    /// <summary>
    /// Resolves a report by marking it as resolved asynchronously.
    /// </summary>
    /// <param name="reportId">The identifier of the report to resolve.</param>
    /// <returns>The resolved report.</returns>
    /// <exception cref="KeyNotFoundException">Thrown when the report with the specified identifier is not found.</exception>
    public async Task<Report> ResolveReportAsync(int reportId)
    {
        var report = await _reportRepository.GetReportByIdAsync(reportId);
        
        if (report == null)
            throw new KeyNotFoundException($"Le signalement avec l'ID {reportId} n'a pas été trouvé.");
        
        report.IsResolved = true;
        report.UpdatedAt = DateTime.UtcNow;
        
        await _reportRepository.UpdateReportAsync(report);
        return report;
    }
    
    /// <summary>
    /// Validates the report data.
    /// </summary>
    /// <param name="report">The report to validate.</param>
    /// <exception cref="ArgumentNullException">Thrown when the report is null.</exception>
    /// <exception cref="ArgumentException">Thrown when the report data is invalid.</exception>
    private void ValidateReport(Report report)
    {
        if (report == null)
            throw new ArgumentNullException(nameof(report));
            
        if (report.UserId <= 0)
            throw new ArgumentException("L'identifiant de l'utilisateur doit être spécifié.", nameof(report.UserId));
            
        if (string.IsNullOrWhiteSpace(report.Comment))
            throw new ArgumentException("Le commentaire ne peut pas être vide.", nameof(report.Comment));
    }
    
    /// <summary>
    /// Gets the total count of reports asynchronously.
    /// </summary>
    /// <returns>The total count of reports.</returns>
    public async Task<int> GetReportsCountAsync()
    {
        return await _reportRepository.GetReportsCountAsync();
    }
}
