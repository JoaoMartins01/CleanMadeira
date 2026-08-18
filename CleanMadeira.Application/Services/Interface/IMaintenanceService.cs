using CleanMadeira.Domain.Entities;

namespace CleanMadeira.Application.Interfaces.Services;

public interface IMaintenanceService
{
    Task<IEnumerable<Maintenance>> GetAllAsync();

    Task<Maintenance?> GetByIdAsync(Guid id);

    Task<Maintenance?> GetByIdAndOwnerIdAsync(Guid id, Guid ownerId);

    Task<IEnumerable<Maintenance>> GetByOwnerIdAsync(Guid ownerId);

    Task<IEnumerable<Maintenance>> GetByAssignedUserIdAsync(Guid userId);

    Task<IEnumerable<Maintenance>> GetByOwnerAndAssignedUserIdAsync(
        Guid ownerId,
        Guid userId);

    Task<IEnumerable<Maintenance>> GetByDateRangeAsync(
        Guid ownerId,
        DateTime startDate,
        DateTime endDate);

    Task CreateAsync(Maintenance maintenance);

    Task<Maintenance?> GetByAccessTokenAsync(Guid token);

    Task UpdateAsync(Maintenance maintenance);

    Task<bool> DeleteAsync(Guid id);

    Task<int> GetPendingCountAsync(Guid ownerId);

    Task<int> GetInProgressCountAsync(Guid ownerId);

    Task<int> GetCompletedCountAsync(Guid ownerId);
}