using Microsoft.EntityFrameworkCore;
using Pawesome.Data;
using Pawesome.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using Pawesome.Interfaces;

namespace Pawesome.Repositories;

public class ReportRepository : IReportRepository
{
    private readonly AppDbContext _context;

    public ReportRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Report>> GetAllReportsAsync()
    {
        return await _context.Reports.ToListAsync();
    }

    public async Task<Report?> GetReportByIdAsync(int id)
    {
        return await _context.Reports.FindAsync(id);
    }

    public async Task AddReportAsync(Report report)
    {
        await _context.Reports.AddAsync(report);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateReportAsync(Report report)
    {
        _context.Reports.Update(report);
        await _context.SaveChangesAsync();
    }
}