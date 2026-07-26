using CleanMadeira.Domain.Entities;

namespace CleanMadeira.Application.Interfaces.Repositories;

public interface IMaintenanceRepository
{
    Task<IEnumerable<Maintenance>> GetAllAsync();

    Task<Maintenance?> GetByIdAsync(Guid id);

    Task<IEnumerable<Maintenance>> GetByPropertyIdAsync(Guid propertyId);

    Task<IEnumerable<Maintenance>> GetByAssignedUserIdAsync(Guid userId);

    Task<IEnumerable<Maintenance>> GetByOwnerIdAsync(Guid ownerId);

    Task<IEnumerable<Maintenance>> GetByOwnerAndAssignedUserIdAsync(Guid ownerId, Guid userId);
    Task<IEnumerable<Maintenance>> GetByDateRangeAsync(
        Guid ownerId,
        DateTime startDate,
        DateTime endDate);
    Task<Maintenance?> GetByAccessTokenAsync(Guid token);
    Task<int> GetPendingCountAsync(Guid ownerId);

    Task<int> GetInProgressCountAsync(Guid ownerId);

    Task<int> GetCompletedCountAsync(Guid ownerId);

    Task AddAsync(Maintenance maintenance);

    void Update(Maintenance maintenance);

    void Delete(Maintenance maintenance);

    Task SaveChangesAsync();
}