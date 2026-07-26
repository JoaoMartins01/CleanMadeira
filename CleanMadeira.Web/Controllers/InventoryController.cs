using CleanMadeira.Application.Services.Implementation;
using CleanMadeira.Application.Services.Interface;
using CleanMadeira.Domain.Entities;
using CleanMadeira.Web.ViewModels;
using CleanMadeira.Web.ViewModels.Inventory;
using CleanMadeira.Web.ViewModels.Propriedade;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Security.Claims;

namespace CleanMadeira.Web.Controllers;

public class InventoryController : Controller
{
    private readonly IInventoryService _inventoryService;
    private readonly ICleaningTaskService _cleaningTaskService;
    private readonly IPropertyService _propertyService;
    private readonly UserManager<ApplicationUser> _userManager;

    public InventoryController(IInventoryService inventoryService,
        ICleaningTaskService cleaningTaskService,
        IPropertyService propriedadeService,
        UserManager<ApplicationUser> userManager)
    {
        _propertyService = propriedadeService;
        _cleaningTaskService = cleaningTaskService;
        _inventoryService = inventoryService;
        _userManager = userManager;
    }

    public async Task<IActionResult> Index(Guid? PropriedadeSelecionadaId)
    {
        var propriedades = await _propertyService.GetAllAsync();

        if (PropriedadeSelecionadaId == null && propriedades.Any())
        {
            PropriedadeSelecionadaId = propriedades.First().Id;
        }

        var produtos = await _inventoryService
            .GetByPropertyIdAsync(PropriedadeSelecionadaId.Value);

        var vm = new InventoryIndexVM
        {
            PropriedadeSelecionadaId = PropriedadeSelecionadaId,

            Propriedades = propriedades
        .Select(p => new SelectListItem
        {
            Value = p.Id.ToString(),
            Text = p.Name
        })
        .ToList(),

            Produtos = produtos.Select(p => new InventoryItemVM
            {
                Id = p.Id,
                Nome = p.Name,
                Quantidade = p.Quantity,
                Unidade = p.Unity,
                QuantidadeMinima = p.MinimumQuantity,
                PropriedadeId = p.PropertyId
            }).ToList()
        };

        return View(vm);
    }

    public async Task<IActionResult> Details(Guid id)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var item = await _inventoryService.GetByIdAndOwnerAsync(id, userId);

        if (item == null)
            return NotFound();

        var vm = new InventoryItemVM
        {
            Id = item.Id,
            PropriedadeId = item.PropertyId,
            PropriedadeNome = item.Property?.Name ?? "",
            Nome = item.Name,
            Unidade = item.Unity,
            Quantidade = item.Quantity,
            QuantidadeMinima = item.MinimumQuantity,
            Active = item.Active
        };

        return View(vm);
    }

    public async Task<IActionResult> CreateAsync()
    {
        await LoadPropriedadesAsync();

        return View(new CreateInventoryItemVM());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateInventoryItemVM vm)
    {
        if (!ModelState.IsValid)
        {
            await LoadPropriedadesAsync();
            return View(vm);
        }

        var item = new InventoryItem
        {
            Id = Guid.NewGuid(),
            PropertyId = vm.PropriedadeId,
            Name = vm.Nome,
            Unity = vm.Unidade,
            Quantity = vm.Quantidade,
            MinimumQuantity = vm.QuantidadeMinima,
            Active = true
        };

        await _inventoryService.CreateAsync(item);

        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(Guid id)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var item = await _inventoryService.GetByIdAndOwnerAsync(id, userId);

        var propriedade = await _propertyService.GetByIdAsync(item.PropertyId);

        if (item == null)
            return NotFound();

        var vm = new InventoryItemVM
        {
            Id = item.Id,
            PropriedadeNome = propriedade.Name,
            Nome = item.Name,
            Unidade = item.Unity,
            Quantidade = item.Quantity,
            QuantidadeMinima = item.MinimumQuantity,
            Active = item.Active
        };

        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Guid id, InventoryItemVM vm)
    {
        if (id != vm.Id)
            return NotFound();

        if (!ModelState.IsValid)
            return View(vm);

        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var item = await _inventoryService.GetByIdAndOwnerAsync(id, userId);

        if (item == null)
            return NotFound();

        item.Name = vm.Nome;
        item.Unity = vm.Unidade;
        item.Quantity = vm.Quantidade;
        item.MinimumQuantity= vm.QuantidadeMinima;
        item.Active = vm.Active;

        await _inventoryService.UpdateAsync(item);

        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Delete(Guid id)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var item = await _inventoryService.GetByIdAndOwnerAsync(id, userId);

        if (item == null)
            return NotFound();

        var vm = new InventoryItemVM
        {
            Id = item.Id,
            Nome = item.Name,
            Quantidade = item.Quantity,
            QuantidadeMinima = item.MinimumQuantity,
            Unidade = item.Unity,
            PropriedadeNome = item.Property?.Name ?? ""
        };

        return View(vm);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(Guid id)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var item = await _inventoryService.GetByIdAndOwnerAsync(id, userId);

        if (item == null)
            return NotFound();

        await _inventoryService.DeleteAsync(id);

        TempData["Success"] = "Produto eliminado com sucesso.";

        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Check(Guid taskId)
    {
        var user = await _userManager.GetUserAsync(User);

        if (user == null)
            return Challenge();

        var task = await _cleaningTaskService
            .GetByIdAsync(taskId);

        if (task == null)
            return NotFound();

        var items = await _inventoryService
            .GetByPropertyIdAsync(task.PropertyId);

        var vm = new InventoryCheckVM
        {
            CleaningTaskId = task.Id,
            PropriedadeId = task.PropertyId,
            PropriedadeNome = task.Property?.Name ?? string.Empty,

            Items = items.Select(i => new InventoryCheckItemVM
            {
                InventoryItemId = i.Id,
                Nome = i.Name,
                QuantidadeAtual = i.Quantity,
                QuantidadeMinima = i.MinimumQuantity,
                Unidade = i.Unity
            }).ToList()
        };

        return View(vm);
    }

    [HttpPost]
    public async Task<IActionResult> Check(InventoryCheckVM vm)
    {
        if (!ModelState.IsValid)
            return View(vm);

        var user = await _userManager.GetUserAsync(User);

        foreach (var item in vm.Items) { 

            var i = await _inventoryService.GetByIdAsync(item.InventoryItemId);

            if (i == null)
                return NotFound();
            
            i.Name = item.Nome;
            i.Quantity = item.QuantidadeAtual;
            i.MinimumQuantity = item.QuantidadeMinima;
            i.Active = true;

            await _inventoryService.UpdateAsync(i);
        }

        TempData["Success"] = "Inventário atualizado com sucesso.";

        return RedirectToAction("Details", "CleaningTask", new
        {
            id = vm.CleaningTaskId
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateCheck(
    Guid taskId,
    Dictionary<Guid, int> quantities)
    {
        foreach (var item in quantities)
        {
            await _inventoryService.UpdateQuantityAsync(
                item.Key,
                item.Value);
        }

        return RedirectToAction("MyTasks", "CleaningTask");
    }

    private async Task LoadPropriedadesAsync()
    {
        var user = await _userManager.GetUserAsync(User);

        var propriedades = await _propertyService
            .GetByUserAsync(user.Id);

        ViewBag.Propriedades = new SelectList(
            propriedades,
            "Id",
            "Name");
    }
}