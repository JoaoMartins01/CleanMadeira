using CleanMadeira.Application.Common.Interfaces;
using CleanMadeira.Domain.Entities;
using Stripe.Checkout;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
