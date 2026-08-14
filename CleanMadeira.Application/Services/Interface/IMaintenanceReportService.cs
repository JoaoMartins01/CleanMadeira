using CleanMadeira.Domain.Entities;

namespace CleanMadeira.Application.Interfaces;

public interface IMaintenanceReportService
{
    Task<IEnumerable<MaintenanceReport>> GetAllAsync();

    Task<IEnumerable<MaintenanceReport>> GetByOwnerIdAsync(Guid ownerId);

    Task<IEnumerable<MaintenanceReport>> GetPendingByOwnerIdAsync(Guid ownerId);

    Task<MaintenanceReport?> GetByIdAsync(Guid id);

    Task AddAsync(MaintenanceReport report);

    Task UpdateAsync(MaintenanceReport report);

    Task DeleteAsync(MaintenanceReport report);

    Task MarkAsConvertedAsync(
        Guid reportId,
        Guid maintenanceId);

    Task MarkAsRejectedAsync(Guid reportId);

    Task MarkAsResolvedWithoutMaintenanceAsync(Guid reportId);
}