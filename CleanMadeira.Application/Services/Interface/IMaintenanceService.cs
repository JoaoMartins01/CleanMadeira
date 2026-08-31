using CleanMadeira.Application.Common.DTO;
using CleanMadeira.Application.Interfaces.Repositories;
using CleanMadeira.Domain.Entities;

namespace CleanMadeira.Application.Interfaces.Services;

public interface IMaintenanceService
{
    Task<IEnumerable<Maintenance>> GetAllAsync();

    Task<Maintenance?> GetByIdAsync(Guid id);

    Task<Maintenance?> GetByIdAndOwnerIdAsync(Guid id, Guid ownerId);

    Task<IEnumerable<Maintenance>> GetByOwnerIdAsync(Guid ownerId);

    Task<IEnumerable<Maintenance>> GetAccessibleMaintenancesAsync(Guid userId, Guid? companyId);

    Task<Maintenance?> GetAccessibleByIdAsync(Guid maintenanceId, Guid userId, Guid? companyId);

    Task<IEnumerable<Maintenance>> GetByAssignedUserIdAsync(Guid userId);

    Task<IEnumerable<Maintenance>> GetByOwnerAndAssignedUserIdAsync(
        Guid ownerId,
        Guid userId);

    Task<IEnumerable<Maintenance>> GetByDateRangeAsync(
        Guid ownerId,
        DateTime startDate,
        DateTime endDate);

    Task<MonthlyMaintenanceReportDto> GetMonthlyReportAsync(
    Guid ownerId,
    int year,
    int month);

    Task CreateAsync(Maintenance maintenance);

    Task<Maintenance?> GetByAccessTokenAsync(Guid token);

    Task UpdateAsync(Maintenance maintenance);

    Task<bool> DeleteAsync(Guid id);

    Task<int> GetPendingCountAsync(Guid ownerId);

    Task<int> GetInProgressCountAsync(Guid ownerId);

    Task<int> GetCompletedCountAsync(Guid ownerId);
}