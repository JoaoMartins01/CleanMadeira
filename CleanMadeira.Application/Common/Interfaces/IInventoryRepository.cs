using CleanMadeira.Domain.Entities;

namespace CleanMadeira.Application.Common.Interfaces
{
    public interface IInventoryRepository : IRepository<InventoryItem>
    {
        Task<IEnumerable<InventoryItem>> GetLowStockAsync();

        Task<IEnumerable<InventoryItem>> GetByPropertyIdAsync(Guid propertyId);

        Task UpdateQuantityAsync(Guid inventoryItemId, int quantity);
        Task<IEnumerable<InventoryItem>> GetByOwnerIdAsync(Guid ownerId);
        Task<InventoryItem?> GetByIdAndOwnerAsync(Guid id, Guid ownerId);
        Task<IEnumerable<InventoryItem>> GetLowStockByOwnerIdAsync(Guid ownerId);
    }
}