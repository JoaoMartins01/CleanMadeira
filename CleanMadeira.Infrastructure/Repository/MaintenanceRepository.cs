using CleanMadeira.Application.Interfaces.Repositories;
using CleanMadeira.Domain.Entities;
using CleanMadeira.Domain.Enums;
using CleanMadeira.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CleanMadeira.Infrastructure.Repositories;

public class MaintenanceRepository : IMaintenanceRepository
{
    private readonly ApplicationDbContext _context;

    public MaintenanceRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Maintenance>> GetAllAsync()
    {
        return await _context.Maintenances
            .Include(x => x.Property)
            .Include(x => x.AssignedUser)
            .Include(x => x.MaintenanceProvider)
            .ToListAsync();
    }

    public async Task<Maintenance?> GetByIdAsync(Guid id)
    {
        return await _context.Maintenances
            .Include(x => x.Property)
            .Include(x => x.AssignedUser)
            .Include(x => x.MaintenanceProvider)
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<Maintenance?> GetByIdAndOwnerIdAsync(Guid id, Guid OwnerId)
    {
        return await _context.Maintenances
            .Include(x => x.Property)
            .Include(x => x.AssignedUser)
            .Include(x => x.MaintenanceProvider)
            .FirstOrDefaultAsync(x =>
                x.Id == id &&
                x.Property.ApplicationUserId == OwnerId);
    }

    public async Task<IEnumerable<Maintenance>> GetByPropertyIdAsync(Guid propertyId)
    {
        return await _context.Maintenances
            .Where(x => x.PropertyId == propertyId)
            .Include(x => x.AssignedUser)
            .ToListAsync();
    }

    public async Task<IEnumerable<Maintenance>> GetByAssignedUserIdAsync(Guid userId)
    {
        return await _context.Maintenances
            .Include(m => m.Property)
            .Include(m => m.AssignedUser)
            .Where(m => m.AssignedUserId == userId)
            .ToListAsync();
    }

    public async Task<IEnumerable<Maintenance>> GetByOwnerIdAsync(Guid ownerId)
    {
        return await _context.Maintenances
            .Include(m => m.Property)
            .Include(m => m.MaintenanceProvider)
            .Where(m => m.Property.ApplicationUserId == ownerId)
            .OrderByDescending(m => m.ScheduledDate)
            .ToListAsync();
    }

    public async Task<IEnumerable<Maintenance>> GetByOwnerAndAssignedUserIdAsync(Guid ownerId, Guid userId)
    {
        return await _context.Maintenances
            .Include(m => m.Property)
            .Include(m => m.AssignedUser)
            .Where(m => m.Property.ApplicationUserId == ownerId &&
                        m.AssignedUserId == userId)
            .ToListAsync();
    }

    public async Task<IEnumerable<Maintenance>> GetByDateRangeAsync(
    Guid ownerId,
    DateTime startDate,
    DateTime endDate)
    {
        return await _context.Maintenances
            .Include(m => m.Property)
            .Include(m => m.AssignedUser)
            .Where(m =>
                m.Property.ApplicationUserId == ownerId &&
                m.ScheduledDate >= startDate &&
                m.ScheduledDate <= endDate)
            .OrderBy(m => m.ScheduledDate)
            .ToListAsync();
    }

    public async Task<int> GetPendingCountAsync(Guid ownerId)
    {
        return await _context.Maintenances
            .Where(m =>
                m.Property.ApplicationUserId == ownerId &&
                m.Status == MaintenanceStatus.Pendente)
            .CountAsync();
    }

    public async Task<int> GetInProgressCountAsync(Guid ownerId)
    {
        return await _context.Maintenances
            .Where(m =>
                m.Property.ApplicationUserId == ownerId &&
                m.Status == MaintenanceStatus.EmProgresso)
            .CountAsync();
    }

    public async Task<int> GetCompletedCountAsync(Guid ownerId)
    {
        return await _context.Maintenances
            .Where(m =>
                m.Property.ApplicationUserId == ownerId &&
                m.Status == MaintenanceStatus.Completo)
            .CountAsync();
    }

    public async Task<Maintenance?> GetByAccessTokenAsync(Guid token)
    {
        return await _context.Maintenances
            .Include(x => x.MaintenanceProvider)
            .Include(x => x.Property)
            .FirstOrDefaultAsync(x => x.AccessToken == token);
    }

    public async Task AddAsync(Maintenance maintenance)
    {
        await _context.Maintenances.AddAsync(maintenance);
    }

    public void Update(Maintenance maintenance)
    {
        _context.Maintenances.Update(maintenance);
    }

    public void Delete(Maintenance maintenance)
    {
        _context.Maintenances.Remove(maintenance);
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}