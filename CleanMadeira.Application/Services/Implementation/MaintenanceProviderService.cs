using CleanMadeira.Application.Interfaces.Repositories;
using CleanMadeira.Application.Interfaces.Services;
using CleanMadeira.Domain.Entities;

namespace CleanMadeira.Application.Services
{
    public class MaintenanceProviderService : IMaintenanceProviderService
    {
        private readonly IMaintenanceProviderRepository _repository;

        public MaintenanceProviderService(
            IMaintenanceProviderRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<MaintenanceProvider>> GetAllAsync()
        {
            return await _repository.GetAllAsync();
        }

        public async Task<IEnumerable<MaintenanceProvider>> GetByOwnerIdAsync(Guid ownerId)
        {
            return await _repository.GetByOwnerIdAsync(ownerId);
        }

        public async Task<MaintenanceProvider?> GetByIdAsync(Guid id)
        {
            return await _repository.GetByIdAsync(id);
        }

        public async Task<MaintenanceProvider?> GetByIdAndOwnerIdAsync(Guid id, Guid ownerId)
        {
            return await _repository.GetByIdAndOwnerIdAsync(id, ownerId);
        }

        public async Task AddAsync(MaintenanceProvider provider)
        {
            provider.Id = Guid.NewGuid();

            provider.Name = provider.Name.Trim();
            provider.Category = provider.Category.Trim();
            provider.Phone = provider.Phone.Trim();

            if (!string.IsNullOrWhiteSpace(provider.Email))
                provider.Email = provider.Email.Trim();

            await _repository.AddAsync(provider);
            await _repository.SaveChangesAsync();
        }

        public async Task UpdateAsync(MaintenanceProvider provider)
        {
            var existing = await _repository.GetByIdAsync(provider.Id);

            if (existing == null)
                throw new Exception("Prestador não encontrado.");

            existing.Name = provider.Name.Trim();
            existing.Category = provider.Category.Trim();
            existing.Phone = provider.Phone.Trim();
            existing.Email = string.IsNullOrWhiteSpace(provider.Email)
                ? null
                : provider.Email.Trim();

            _repository.Update(existing);

            await _repository.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var provider = await _repository.GetByIdAsync(id);

            if (provider == null)
                throw new Exception("Prestador não encontrado.");

            _repository.Delete(provider);

            await _repository.SaveChangesAsync();
        }
    }
}