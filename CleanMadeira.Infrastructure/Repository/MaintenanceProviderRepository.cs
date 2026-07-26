using CleanMadeira.Application.Interfaces.Repositories;
using CleanMadeira.Domain.Entities;
using CleanMadeira.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CleanMadeira.Infrastructure.Repositories
{
    public class MaintenanceProviderRepository : IMaintenanceProviderRepository
    {
        private readonly ApplicationDbContext _context;

        public MaintenanceProviderRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<MaintenanceProvider>> GetAllAsync()
        {
            return await _context.MaintenanceProviders
                .Include(x => x.Owner)
                .OrderBy(x => x.Name)
                .ToListAsync();
        }

        public async Task<IEnumerable<MaintenanceProvider>> GetByOwnerIdAsync(Guid ownerId)
        {
            return await _context.MaintenanceProviders
                .Where(x => x.OwnerId == ownerId
                && x.Active)
                .OrderBy(x => x.Name)
                .ToListAsync();
        }

        public async Task<MaintenanceProvider?> GetByIdAsync(Guid id)
        {
            return await _context.MaintenanceProviders
                .Include(x => x.Owner)
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task AddAsync(MaintenanceProvider provider)
        {
            await _context.MaintenanceProviders.AddAsync(provider);
        }

        public void Update(MaintenanceProvider provider)
        {
            _context.MaintenanceProviders.Update(provider);
        }

        public void Delete(MaintenanceProvider provider)
        {
            _context.MaintenanceProviders.Remove(provider);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}