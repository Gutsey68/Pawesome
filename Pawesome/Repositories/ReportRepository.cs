using Microsoft.EntityFrameworkCore;
using Pawesome.Data;
using Pawesome.Interfaces;
using Pawesome.Models.Entities;

namespace Pawesome.Repositories;

/// <summary>
/// Repository for managing report entities in the database.
/// </summary>
public class ReportRepository : IReportRepository
{
    private readonly AppDbContext _context;

    /// <summary>
    /// Initializes a new instance of the <see cref="ReportRepository"/> class.
    /// </summary>
    /// <param name="context">The application's database context.</param>
    public ReportRepository(AppDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Retrieves all reports from the database asynchronously.
    /// </summary>
    /// <returns>A collection of all reports.</returns>
    public async Task<IEnumerable<Report>> GetAllReportsAsync()
    {
        return await _context.Reports
            .Include(r => r.User)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync();
    }

    /// <summary>
    /// Retrieves a report by its unique identifier asynchronously.
    /// </summary>
    /// <param name="id">The unique identifier of the report.</param>
    /// <returns>The report if found; otherwise, null.</returns>
    public async Task<Report?> GetReportByIdAsync(int id)
    {
        return await _context.Reports.FindAsync(id);
    }

    /// <summary>
    /// Adds a new report to the database asynchronously.
    /// </summary>
    /// <param name="report">The report entity to add.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task AddReportAsync(Report report)
    {
        await _context.Reports.AddAsync(report);
        await _context.SaveChangesAsync();
    }

    /// <summary>
    /// Updates an existing report in the database asynchronously.
    /// </summary>
    /// <param name="report">The report entity to update.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task UpdateReportAsync(Report report)
    {
        _context.Reports.Update(report);
        await _context.SaveChangesAsync();
    }
    
    /// <summary>
    /// Retrieves the total number of reports in the database asynchronously.
    /// </summary>
    /// <returns>The total count of reports.</returns>
    public async Task<int> GetReportsCountAsync()
    {
        return await _context.Reports.CountAsync();
    }
}
