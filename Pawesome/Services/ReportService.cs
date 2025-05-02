using Pawesome.Interfaces;
using Pawesome.Models.Entities;

namespace Pawesome.Services;

public class ReportService : IReportService
{
    private readonly IReportRepository _reportRepository;

    public ReportService(IReportRepository reportRepository)
    {
        _reportRepository = reportRepository;
    }

    public async Task<IEnumerable<Report>> GetAllReportsAsync()
    {
        return await _reportRepository.GetAllReportsAsync();
    }

    public async Task<Report?> GetReportByIdAsync(int id)
    {
        if (id <= 0)
            throw new ArgumentException("L'identifiant du signalement doit être positif.", nameof(id));

        return await _reportRepository.GetReportByIdAsync(id);
    }

    public async Task<Report> CreateReportAsync(Report report)
    {
        ValidateReport(report);
        
        report.CreatedAt = DateTime.UtcNow;
        report.UpdatedAt = DateTime.UtcNow;
        
        await _reportRepository.AddReportAsync(report);
        return report;
    }

    public async Task<Report> UpdateReportAsync(Report report)
    {
        if (report.Id <= 0)
            throw new ArgumentException("L'identifiant du signalement doit être positif.", nameof(report.Id));
            
        ValidateReport(report);
        
        report.UpdatedAt = DateTime.UtcNow;
        
        await _reportRepository.UpdateReportAsync(report);
        return report;
    }
    
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
    
    private void ValidateReport(Report report)
    {
        if (report == null)
            throw new ArgumentNullException(nameof(report));
            
        if (report.UserId <= 0)
            throw new ArgumentException("L'identifiant de l'utilisateur doit être spécifié.", nameof(report.UserId));
            
        if (string.IsNullOrWhiteSpace(report.Comment))
            throw new ArgumentException("Le commentaire ne peut pas être vide.", nameof(report.Comment));
    }
}