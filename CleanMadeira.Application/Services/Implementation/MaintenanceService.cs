using CleanMadeira.Application.Interfaces.Repositories;
using CleanMadeira.Application.Interfaces.Services;
using CleanMadeira.Domain.Entities;

namespace CleanMadeira.Application.Services;

public class MaintenanceService : IMaintenanceService
{
    private readonly IMaintenanceRepository _maintenanceRepository;

    public MaintenanceService(
        IMaintenanceRepository maintenanceRepository)
    {
        _maintenanceRepository = maintenanceRepository;
    }

    public async Task<IEnumerable<Maintenance>> GetAllAsync()
    {
        return await _maintenanceRepository.GetAllAsync();
    }

    public async Task<Maintenance?> GetByIdAsync(Guid id)
    {
        return await _maintenanceRepository.GetByIdAsync(id);
    }

    public async Task<Maintenance?> GetByIdAndOwnerIdAsync(Guid id, Guid ownerId)
    {
        return await _maintenanceRepository.GetByIdAndOwnerIdAsync(id, ownerId);
    }

    public async Task<IEnumerable<Maintenance>> GetByOwnerIdAsync(
        Guid ownerId)
    {
        return await _maintenanceRepository.GetByOwnerIdAsync(ownerId);
    }

    public async Task<IEnumerable<Maintenance>> GetByAssignedUserIdAsync(
        Guid userId)
    {
        return await _maintenanceRepository
            .GetByAssignedUserIdAsync(userId);
    }

    public async Task<IEnumerable<Maintenance>>
        GetByOwnerAndAssignedUserIdAsync(
            Guid ownerId,
            Guid userId)
    {
        return await _maintenanceRepository
            .GetByOwnerAndAssignedUserIdAsync(ownerId, userId);
    }

    public async Task<IEnumerable<Maintenance>> GetByDateRangeAsync(
        Guid ownerId,
        DateTime startDate,
        DateTime endDate)
    {
        if (endDate < startDate)
        {
            throw new ArgumentException(
                "A data final não pode ser anterior à data inicial.");
        }

        return await _maintenanceRepository.GetByDateRangeAsync(
            ownerId,
            startDate,
            endDate);
    }

    public async Task<Maintenance?> GetByAccessTokenAsync(Guid token)
    {
        return await _maintenanceRepository.GetByAccessTokenAsync(token);
    }

    public async Task CreateAsync(Maintenance maintenance)
    {
        ArgumentNullException.ThrowIfNull(maintenance);

        maintenance.Id = maintenance.Id == Guid.Empty
            ? Guid.NewGuid()
            : maintenance.Id;

        maintenance.Title = maintenance.Title.Trim();
        maintenance.Description = maintenance.Description.Trim();
        maintenance.CreatedAt = DateTime.UtcNow;

        await _maintenanceRepository.AddAsync(maintenance);
        await _maintenanceRepository.SaveChangesAsync();
    }

    public async Task UpdateAsync(Maintenance maintenance)
    {
        ArgumentNullException.ThrowIfNull(maintenance);

        var existingMaintenance =
            await _maintenanceRepository.GetByIdAsync(maintenance.Id);

        if (existingMaintenance is null)
        {
            throw new KeyNotFoundException(
                "A manutenção indicada não foi encontrada.");
        }

        existingMaintenance.PropertyId = maintenance.PropertyId;
        existingMaintenance.AssignedUserId =
            maintenance.AssignedUserId;

        existingMaintenance.Title = maintenance.Title.Trim();
        existingMaintenance.Description =
            maintenance.Description.Trim();

        existingMaintenance.Priority = maintenance.Priority;
        existingMaintenance.Status = maintenance.Status;
        existingMaintenance.ScheduledDate =
            maintenance.ScheduledDate;

        _maintenanceRepository.Update(existingMaintenance);
        await _maintenanceRepository.SaveChangesAsync();
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var maintenance =
            await _maintenanceRepository.GetByIdAsync(id);

        if (maintenance is null)
        {
            return false;
        }

        _maintenanceRepository.Delete(maintenance);
        await _maintenanceRepository.SaveChangesAsync();

        return true;
    }



    public async Task<int> GetPendingCountAsync(Guid ownerId)
    {
        return await _maintenanceRepository
            .GetPendingCountAsync(ownerId);
    }

    public async Task<int> GetInProgressCountAsync(Guid ownerId)
    {
        return await _maintenanceRepository
            .GetInProgressCountAsync(ownerId);
    }

    public async Task<int> GetCompletedCountAsync(Guid ownerId)
    {
        return await _maintenanceRepository
            .GetCompletedCountAsync(ownerId);
    }
}