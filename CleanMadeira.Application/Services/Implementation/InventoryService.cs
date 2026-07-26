using CleanMadeira.Application.Common.Interfaces;
using CleanMadeira.Application.Services.Interface;
using CleanMadeira.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
public class InventoryService : IInventoryService
{
    private readonly IInventoryRepository _inventoryRepository;

    public InventoryService(IInventoryRepository inventoryRepository)
    {
        _inventoryRepository = inventoryRepository;
    }

    public async Task<IEnumerable<InventoryItem>> GetAllAsync()
    {
        return await _inventoryRepository.GetAllAsync();
    }

    public async Task<InventoryItem?> GetByIdAsync(Guid id)
    {
        return await _inventoryRepository.GetByIdAsync(id);
    }

    public async Task<IEnumerable<InventoryItem>> GetLowStockAsync()
    {
        return await _inventoryRepository.GetLowStockAsync();
    }

    public async Task CreateAsync(InventoryItem item)
    {
        item.Id = Guid.NewGuid();
        item.Active = true;
        item.CreatedAt = DateTime.UtcNow;

        await _inventoryRepository.AddAsync(item);
        await _inventoryRepository.SaveChangesAsync();
    }

    public async Task UpdateAsync(InventoryItem item)
    {
        await _inventoryRepository.UpdateAsync(item);
        await _inventoryRepository.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        var item = await _inventoryRepository.GetByIdAsync(id);

        if (item == null)
            return;

        await _inventoryRepository.DeleteAsync(item);
        await _inventoryRepository.SaveChangesAsync();
    }

    public async Task AddStockAsync(Guid id, int quantity)
    {
        var item = await _inventoryRepository.GetByIdAsync(id);

        if (item == null)
            return;

        item.Quantity += quantity;

        await _inventoryRepository.UpdateAsync(item);
        await _inventoryRepository.SaveChangesAsync();
    }

    public async Task<IEnumerable<InventoryItem>> GetByPropertyIdAsync(Guid propertyId)
    {
        return await _inventoryRepository.GetByPropertyIdAsync(propertyId);
    }

    public async Task UpdateQuantityAsync(Guid inventoryItemId, int quantity)
    {
        await _inventoryRepository.UpdateQuantityAsync(inventoryItemId, quantity);
    }

    public async Task<IEnumerable<InventoryItem>> GetByOwnerIdAsync(Guid ownerId)
    {
        return await _inventoryRepository.GetByOwnerIdAsync(ownerId);
    }

    public async Task<InventoryItem?> GetByIdAndOwnerAsync(Guid id, Guid ownerId)
    {
        return await _inventoryRepository.GetByIdAndOwnerAsync(id, ownerId);
    }

    public async Task<IEnumerable<InventoryItem>> GetLowStockByOwnerIdAsync(Guid ownerId)
    {
        return await _inventoryRepository.GetLowStockByOwnerIdAsync(ownerId);
    }

    /*public async Task CheckInventoryAsync(InventoryCheckVM vm, Guid userId)
    {
        // Obter a limpeza
        var task = await _cleaningTaskRepositorio.GetByIdAsync(vm.CleaningTaskId);

        if (task == null)
            throw new Exception("Limpeza não encontrada.");

        foreach (var itemVm in vm.Items)
        {
            var item = await _inventoryRepository.GetByIdAsync(itemVm.InventoryItemId);

            if (item == null)
                continue;

            // Segurança: impedir alterar produtos de outra propriedade
            if (item.PropriedadeId != task.PropriedadeId)
                continue;

            // Atualizar quantidade
            item.Quantidade = itemVm.QuantidadeAtual;

            await _inventoryRepository.UpdateAsync(item);

            // Criar ou remover alerta
            if (item.Quantidade <= item.QuantidadeMínima)
            {
                await _inventoryAlertService.CreateOrUpdateAsync(item.Id);
            }
            else
            {
                await _inventoryAlertService.RemoveByInventoryItemIdAsync(item.Id);
            }
        }

        task.InventoryChecked = true;
        task.InventoryCheckedAt = DateTime.UtcNow;
        task.InventoryCheckedById = userId;

        await _cleaningTaskRepository.UpdateAsync(task);

        await _unitOfWork.SaveChangesAsync();
    }*/
}