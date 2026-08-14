using CleanMadeira.Domain.Entities;

namespace CleanMadeira.Application.Services.Interface
{
    public interface IInventoryService : IService<InventoryItem>
    {
        Task<IEnumerable<InventoryItem>> GetLowStockAsync();
        Task AddStockAsync(Guid id, int quantity);

        Task<IEnumerable<InventoryItem>> GetByPropertyIdAsync(Guid propertyId);

        Task UpdateQuantityAsync(Guid inventoryItemId, int quantity);
        Task<IEnumerable<InventoryItem>> GetByOwnerIdAsync(Guid ownerId);
        Task<InventoryItem?> GetByIdAndOwnerAsync(Guid id, Guid ownerId);
        Task<IEnumerable<InventoryItem>> GetLowStockByOwnerIdAsync(Guid ownerId);
    }
}
