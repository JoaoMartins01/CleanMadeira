using CleanMadeira.Application.Services.Interface;
using CleanMadeira.Domain.Entities;
using CleanMadeira.Web.ViewModels;
using CleanMadeira.Web.ViewModels.CleaningTask;
using CleanMadeira.Web.ViewModels.Propriedade;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;
using System.Security.Claims;

namespace CleanMadeira.Web.Controllers;

public class PropriedadeController : Controller
{
    private readonly IPropertyService _propertyService;
    private readonly ICleaningTaskService _cleaningTaskService;
    private readonly IInventoryService _inventoryService;
    private readonly ICalendarIntegrationService _calendarIntegrationService;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;

    public PropriedadeController(IPropertyService propertyService,
        ICleaningTaskService cleaningTaskService,
        IInventoryService inventoryService,
        ICalendarIntegrationService calendarIntegrationService,
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager)
    {
        _propertyService = propertyService;
        _cleaningTaskService = cleaningTaskService;
        _calendarIntegrationService = calendarIntegrationService;
        _userManager = userManager;
        _signInManager = signInManager;
    }

    public async Task<IActionResult> Index()
    {
        var user = await _userManager.GetUserAsync(User);

        var properties = await _propertyService.GetByUserAsync(user.Id);

        ViewBag.InactiveCount = await _propertyService.CountInactiveAsync(user.Id);

        properties = properties
            .Where(p => p.Active)
            .ToList();

        var vm = properties.Select(p => new PropriedadeVM
        {
            Id = p.Id,
            Nome = p.Name,
            Endereco = p.Address,
            Freguesia = p.Freguesia,
            Quartos = p.Rooms,
            CasasBanho = p.Bathrooms,
            Active = p.Active
        });

        return View(vm);
    }

    [HttpGet]
    public async Task<IActionResult> Details(Guid id)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var property = await _propertyService.GetByIdAndOwnerAsync(id, userId);

        if (property == null)
            return NotFound();

        if (property.ApplicationUserId != userId)
            return Forbid();

        var calendarIntegrations =
            await _calendarIntegrationService.GetByPropertyIdAsync(id, userId);

        var cleaningTasks =
            await _cleaningTaskService.GetByOwnerIdAsync(userId);

        var inventory = 0;

        var nextCleaning =
            cleaningTasks
                .Where(x => x.ScheduledDate >= DateTime.Today &&
                       x.PropertyId == id)
                .OrderBy(x => x.ScheduledDate)
                .FirstOrDefault();

        var model = new PropriedadeDetailsVM
        {
            Propriedade = new PropriedadeVM
            {
                Id = property.Id,
                Nome = property.Name,
                Endereco = property.Address
            },

            CalendarCount = calendarIntegrations.Count,

            InventoryCount = 2,

            PhotoCount = 0, // alterar quando implementares as fotografias

            NextCleaningTask = nextCleaning == null
                ? null
                : new CleaningTaskVM
                {
                    Id = nextCleaning.Id,
                    ScheduledDate = nextCleaning.ScheduledDate,
                    Status = nextCleaning.Status
                },

            CalendarIntegrations = calendarIntegrations
                .Select(x => new CalendarIntegrationVM
                {
                    Id = x.Id,
                    Provider = x.Provider,
                    CalendarUrl = x.CalendarUrl,
                    IsEnabled = x.IsEnabled,
                    LastSync = x.LastSync
                })
                .ToList()
        };
        return View(model);
    }

    public IActionResult Create()
    {
        return View(new PropriedadeVM());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(PropriedadeVM vm)
    {
        var user = await _userManager.GetUserAsync(User);

        var exists = await _propertyService.ExistsAsync(vm.Nome,
             user.Id);

        if (vm.Latitude == null || vm.Longitude == null)
        {
            ModelState.AddModelError(
                "",
                "Selecione a localização da property no mapa.");
        }

        if (exists)
        {
            ModelState.AddModelError(
                "Nome",
                "Já existe uma property com este nome.");
        }

        if (!ModelState.IsValid)
            return View(vm);

        var property = new Property
        {
            Id = Guid.NewGuid(),
            Name = vm.Nome,
            PostalCode = vm.CodigoPostal,
            Address = vm.Endereco,
            Freguesia = vm.Freguesia,
            Latitude = vm.Latitude,
            Longitude = vm.Longitude,
            Rooms = vm.Quartos,
            Bathrooms = vm.CasasBanho,
            NumberGuests = vm.NumeroHospedes,
            Description = vm.Descricao,
            ApplicationUserId = user.Id,
            Active = true
        };

        await _propertyService.CreateAsync(property);

        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(Guid id)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var property = await _propertyService.GetByIdAndOwnerAsync(id, userId);

        if (property == null)
            return NotFound();




        var vm = new PropriedadeVM
        {
            Id = property.Id,
            Nome = property.Name,
            Endereco = property.Address,
            Freguesia = property.Freguesia,
            CodigoPostal = property.PostalCode,
            Latitude = property.Latitude,
            Longitude = property.Longitude,
            CasasBanho = property.Bathrooms,
            Quartos = property.Rooms,
            NumeroHospedes = property.NumberGuests,
            Descricao = property.Description,
            Active = property.Active
        };

        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Guid id, PropriedadeVM vm)
    {
        if (id != vm.Id)
            return NotFound();

        if (!ModelState.IsValid)
            return View(vm);

        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var property = await _propertyService.GetByIdAndOwnerAsync(id, userId);

        if (property == null)
            return NotFound();

        property.Name = vm.Nome;
        property.Address = vm.Endereco;
        property.Freguesia = vm.Freguesia;
        property.PostalCode = vm.CodigoPostal;
        property.Latitude = vm.Latitude;
        property.Longitude = vm.Longitude;
        property.Bathrooms = vm.CasasBanho;
        property.Rooms = vm.Quartos;
        property.NumberGuests = vm.NumeroHospedes;
        property.Description = vm.Descricao;
        property.Active = vm.Active;

        await _propertyService.UpdateAsync(property);

        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Delete(Guid id)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var property = await _propertyService.GetByIdAndOwnerAsync(id, userId);

        if (property == null)
            return NotFound();

        var vm = new PropriedadeVM
        {
            Id = property.Id,
            Nome = property.Name,
            Endereco = property.Address,
            Active = property.Active
        };

        return View(vm);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(Guid id)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var property = await _propertyService.GetByIdAndOwnerAsync(id, userId);

        if (property == null)
            return NotFound();

        var hasOpenTasks = await _cleaningTaskService
            .HasOpenTasksByPropertyIdAsync(id);

        if (hasOpenTasks)
        {
            TempData["Error"] =
                "Não é possível desativar esta propriedade porque existem limpezas pendentes ou em progresso.";

            return RedirectToAction(nameof(Index));
        }

        property.Active = false;

        await _propertyService.UpdateAsync(property);

        TempData["Success"] = "Propriedade desativada com sucesso.";

        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Inactive()
    {
        var user = await _userManager.GetUserAsync(User);

        var propertys = await _propertyService.GetInactiveAsync(user.Id);

        var vm = propertys.Select(p => new PropriedadeVM
        {
            Id = p.Id,
            Nome = p.Name,
            Endereco = p.Address,
            Freguesia = p.Freguesia
        });

        return View(vm);
    }

    public async Task<IActionResult> Restore(Guid id)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var property = await _propertyService.GetByIdAndOwnerAsync(id, userId);

        if (property == null)
            return NotFound();

        var vm = new PropriedadeVM
        {
            Id = property.Id,
            Nome = property.Name,
            Endereco = property.Address,
            Freguesia = property.Freguesia
        };

        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> RestoreConfirmed(Guid id)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var property = await _propertyService.GetByIdAndOwnerAsync(id, userId);

        if (property == null)
            return NotFound();

        property.Active = true;

        await _propertyService.UpdateAsync(property);

        TempData["Success"] = "Propriedade reativada com sucesso.";

        return RedirectToAction(nameof(Inactive));
    }
}
