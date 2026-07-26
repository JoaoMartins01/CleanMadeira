using CleanMadeira.Domain.Entities;

namespace CleanMadeira.Application.Interfaces.Services
{
    public interface IMaintenanceProviderService
    {
        Task<IEnumerable<MaintenanceProvider>> GetAllAsync();

        Task<IEnumerable<MaintenanceProvider>> GetByOwnerIdAsync(Guid ownerId);

        Task<MaintenanceProvider?> GetByIdAsync(Guid id);

        Task AddAsync(MaintenanceProvider provider);

        Task UpdateAsync(MaintenanceProvider provider);

        Task DeleteAsync(Guid id);
    }
}
