using CleanMadeira.Application.Common.Interfaces;
using CleanMadeira.Domain.Entities;
using CleanMadeira.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CleanMadeira.Infrastructure.Repository
{
    public class InventoryRepository
        : Repository<InventoryItem>, IInventoryRepository
    {
        public InventoryRepository(ApplicationDbContext context)
            : base(context)
        {
        }

        public async Task<IEnumerable<InventoryItem>> GetLowStockAsync()
        {
            return await _context.InventoryItems
                .Where(x => x.Quantity <= x.MinimumQuantity)
                .ToListAsync();
        }

        public async Task<IEnumerable<InventoryItem>> GetByPropertyIdAsync(Guid propertyId)
        {
            return await _context.InventoryItems
                .Where(x => x.PropertyId == propertyId)
                .ToListAsync();
        }

        public async Task UpdateQuantityAsync(Guid inventoryItemId, int quantity)
        {
            var item = await _context.InventoryItems.FindAsync(inventoryItemId);

            if (item == null)
                return;

            item.Quantity = quantity;

            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<InventoryItem>> GetAllAsync()
        {
            return await _context.InventoryItems
                .Include(i => i.Property)
                .ToListAsync();
        }

        public async Task<IEnumerable<InventoryItem>> GetByOwnerIdAsync(Guid ownerId)
        {
            return await _context.InventoryItems
                .Include(i => i.Property)
                .Where(i => i.Property.ApplicationUserId == ownerId &&
                            i.Active &&
                            i.Property.Active)
                .OrderBy(i => i.Property.Name)
                .ThenBy(i => i.Name)
                .ToListAsync();
        }

        public async Task<InventoryItem?> GetByIdAndOwnerAsync(Guid id, Guid ownerId)
        {
            return await _context.InventoryItems
                .FirstOrDefaultAsync(x =>
                    x.Id == id &&
                    x.Property.ApplicationUserId == ownerId);
        }

        public async Task<IEnumerable<InventoryItem>> GetAccessibleInventoryAsync(
            Guid userId,
            Guid? companyId)
        {
            var query = _context.InventoryItems
                .AsNoTracking()
                .Include(x => x.Property)
                .AsQueryable();

            if (companyId.HasValue)
            {
                query = query.Where(x =>
                    x.Property.CompanyId == companyId.Value);
            }
            else
            {
                query = query.Where(x =>
                    x.Property.ApplicationUserId == userId &&
                    x.Property.CompanyId == null);
            }

            return await query
                .OrderBy(x => x.Property.Name)
                .ThenBy(x => x.Name)
                .ToListAsync();
        }

        public async Task<InventoryItem?> GetAccessibleByIdAsync(
            Guid inventoryItemId,
            Guid userId,
            Guid? companyId)
        {
            var query = _context.InventoryItems
                .AsNoTracking()
                .Include(x => x.Property)
                .AsQueryable();

            if (companyId.HasValue)
            {
                query = query.Where(x =>
                    x.Property.CompanyId == companyId.Value);
            }
            else
            {
                query = query.Where(x =>
                    x.Property.ApplicationUserId == userId &&
                    x.Property.CompanyId == null);
            }

            return await query
                .FirstOrDefaultAsync(x => x.Id == inventoryItemId);
        }

        public async Task<IEnumerable<InventoryItem>> GetLowStockByOwnerIdAsync(Guid ownerId)
        {
            return await _context.InventoryItems
                .Include(i => i.Property)
                .Where(i => i.Property.ApplicationUserId == ownerId &&
                            i.Property.Active &&
                            i.Quantity <= i.MinimumQuantity)
                .OrderBy(i => i.Property.Name)
                .ThenBy(i => i.Name)
                .ToListAsync();
        }

        public async Task<List<InventoryItem>> GetReportItemsAsync(
        Guid ownerId)
        {
            return await _context.InventoryItems
                .AsNoTracking()
                .Include(x => x.Property)
                .Where(x =>
                    x.Property.ApplicationUserId == ownerId)
                .OrderBy(x => x.Property.Name)
                .ThenBy(x => x.Name)
                .ToListAsync();
        }
    }
}
