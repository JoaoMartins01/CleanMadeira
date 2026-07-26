using CleanMadeira.Domain.Entities;

namespace CleanMadeira.Application.Interfaces.Repositories
{
    public interface IMaintenanceProviderRepository
    {
        Task<IEnumerable<MaintenanceProvider>> GetAllAsync();

        Task<IEnumerable<MaintenanceProvider>> GetByOwnerIdAsync(Guid ownerId);

        Task<MaintenanceProvider?> GetByIdAsync(Guid id);

        Task AddAsync(MaintenanceProvider provider);

        void Update(MaintenanceProvider provider);

        void Delete(MaintenanceProvider provider);

        Task SaveChangesAsync();
    }
}