using CleanMadeira.Application.Interfaces.Services;
using CleanMadeira.Domain.Entities;
using CleanMadeira.Web.ViewModels.MaintenanceProvider;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CleanMadeira.Web.Controllers
{
    public class MaintenanceProviderController : Controller
    {
        private readonly IMaintenanceProviderService _maintenanceProviderService;

        public MaintenanceProviderController(IMaintenanceProviderService maintenanceProviderService)
        {
            _maintenanceProviderService = maintenanceProviderService;
        }

        public async Task<IActionResult> Index()
        {
            var ownerId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            var providers = await _maintenanceProviderService.GetByOwnerIdAsync(ownerId);

            var model = providers.Select(x => new MaintenanceProviderVM
            {
                Id = x.Id,
                Name = x.Name,
                Specialty = x.Category,
                PhoneNumber = x.Phone,
                Email = x.Email,
                Active = x.Active,
                Notes = x.Notes,
            });

            return View(model);
        }

        public IActionResult Create()
        {
            return View(new MaintenanceProviderVM());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(MaintenanceProviderVM model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var ownerId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            var provider = new MaintenanceProvider
            {
                Name = model.Name,
                Category = model.Specialty,
                Phone = model.PhoneNumber,
                Email = model.Email,
                Active = model.Active,
                Notes = model.Notes,
                OwnerId = ownerId,
            };

            await _maintenanceProviderService.AddAsync(provider);

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(Guid id)
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            var provider = await _maintenanceProviderService.GetByIdAndOwnerIdAsync(id, userId);

            if (provider == null)
                return NotFound();

            var model = new MaintenanceProviderVM
            {
                Id = provider.Id,
                Name = provider.Name,
                Specialty = provider.Category,
                PhoneNumber = provider.Phone,
                Email = provider.Email,
                Active = provider.Active,
                Notes = provider.Notes
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, MaintenanceProviderVM model)
        {
            if (id != model.Id)
                return NotFound();

            if (!ModelState.IsValid)
                return View(model);

            var provider = await _maintenanceProviderService.GetByIdAsync(id);

            if (provider == null)
                return NotFound();

            provider.Name = model.Name;
            provider.Category = model.Specialty;
            provider.Phone = model.PhoneNumber;
            provider.Email = model.Email;
            provider.Active = model.Active;
            provider.Notes = model.Notes;

            await _maintenanceProviderService.UpdateAsync(provider);

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(Guid id)
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            var provider = await _maintenanceProviderService.GetByIdAndOwnerIdAsync(id, userId);

            if (provider == null)
                return NotFound();

            var model = new MaintenanceProviderVM
            {
                Id = provider.Id,
                Name = provider.Name,
                Specialty = provider.Category,
                PhoneNumber = provider.Phone,
                Email = provider.Email,
                Active = provider.Active,
                Notes = provider.Notes
            };

            return View(model);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var provider = await _maintenanceProviderService.GetByIdAsync(id);

            if (provider == null)
                return NotFound();

            provider.Active = false;
            await _maintenanceProviderService.UpdateAsync(provider);

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Details(Guid id)
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            var provider = await _maintenanceProviderService.GetByIdAndOwnerIdAsync(id, userId);

            if (provider == null)
                return NotFound();

            var model = new MaintenanceProviderVM
            {
                Id = provider.Id,
                Name = provider.Name,
                Specialty = provider.Category,
                PhoneNumber = provider.Phone,
                Email = provider.Email,
                Active = provider.Active,
                Notes = provider.Notes
            };

            return View(model);
        }
    }
}