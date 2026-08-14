using CleanMadeira.Domain.Entities;

namespace CleanMadeira.Domain.Interfaces;

public interface IMaintenanceReportRepository
{
    Task<IEnumerable<MaintenanceReport>> GetAllAsync();

    Task<IEnumerable<MaintenanceReport>> GetByOwnerIdAsync(
        Guid ownerId);

    Task<MaintenanceReport?> GetByIdAsync(Guid id);

    Task AddAsync(MaintenanceReport report);

    Task UpdateAsync(MaintenanceReport report);

    Task DeleteAsync(MaintenanceReport report);
}