using CleanMadeira.Domain.Entities;
using CleanMadeira.Domain.Interfaces;
using CleanMadeira.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CleanMadeira.Infrastructure.Repositories;

public class MaintenanceReportRepository
    : IMaintenanceReportRepository
{
    private readonly ApplicationDbContext _context;

    public MaintenanceReportRepository(
        ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<MaintenanceReport>> GetAllAsync()
    {
        return await _context.MaintenanceReports
            .Include(x => x.Property)
            .Include(x => x.CleaningTask)
            .Include(x => x.ReportedByUser)
            .Include(x => x.Maintenance)
            .ToListAsync();
    }

    public async Task<IEnumerable<MaintenanceReport>>
        GetByOwnerIdAsync(Guid ownerId)
    {
        return await _context.MaintenanceReports
            .Include(x => x.Property)
            .Include(x => x.CleaningTask)
            .Include(x => x.ReportedByUser)
            .Include(x => x.Maintenance)
            .Where(x => x.Property.ApplicationUserId == ownerId)
            .OrderByDescending(x => x.ReportedAt)
            .ToListAsync();
    }

    public async Task<MaintenanceReport?> GetByIdAsync(Guid id)
    {
        return await _context.MaintenanceReports
            .Include(x => x.Property)
            .Include(x => x.CleaningTask)
            .Include(x => x.ReportedByUser)
            .Include(x => x.Maintenance)
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task AddAsync(MaintenanceReport report)
    {
        await _context.MaintenanceReports.AddAsync(report);

        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(MaintenanceReport report)
    {
        _context.MaintenanceReports.Update(report);

        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(MaintenanceReport report)
    {
        _context.MaintenanceReports.Remove(report);

        await _context.SaveChangesAsync();
    }
}