using CleanMadeira.Application.Contract;
using CleanMadeira.Application.Interfaces.Services;
using CleanMadeira.Application.Services;
using CleanMadeira.Application.Services.Interface;
using CleanMadeira.Domain.Entities;
using CleanMadeira.Domain.Entities.Enums;
using CleanMadeira.Web.ViewModels.CleaningTask;
using CleanMadeira.Web.ViewModels.Maintenance;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System.Security.Claims;
using System.Security.Policy;

namespace CleanMadeira.Web.Controllers;

[Authorize]
public class MaintenanceController : Controller
{
    private readonly IMaintenanceService _maintenanceService;
    private readonly IPropertyService _propertyService;
    private readonly IMaintenanceProviderService _maintenanceProviderService;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IEmailService _emailService;

    public MaintenanceController(IMaintenanceService maintenanceService,
        IPropertyService propertyService,
        IMaintenanceProviderService maintenanceProviderService,
        UserManager<ApplicationUser> userManager,
        IEmailService emailService)
    {
        _maintenanceService = maintenanceService;
        _propertyService = propertyService;
        _maintenanceProviderService = maintenanceProviderService;
        _userManager = userManager;
        _emailService = emailService;
    }

    public async Task<IActionResult> Index()
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var model = await _maintenanceService.GetByOwnerIdAsync(userId);

        return View(model);
    }

    public async Task<IActionResult> Details(Guid id)
    {
        var maintenance = await _maintenanceService.GetByIdAsync(id);

        if (maintenance == null)
            return NotFound();

        var vm = new MaintenanceDetailsVM
        {
            Id = maintenance.Id,
            PropertyId = maintenance.PropertyId,
            AssignedUserId = maintenance.MaintenanceProviderId,
            PropertyName = maintenance.Property.Name,
            PropertyAddress = maintenance.Property.Address,
            AssignedUserName = maintenance.MaintenanceProvider.Name,
            Title = maintenance.Title,
            Description = maintenance.Description,
            Priority = maintenance.Priority,
            Status = maintenance.Status,
            ScheduledDate = maintenance.ScheduledDate,
            CreatedAt = maintenance.CreatedAt,
        };    


        return View(vm);
    }

    [HttpGet]
    public async Task<IActionResult> Create()
    {
        await LoadDropdowns();

        return View(new CreateMaintenanceVM());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateMaintenanceVM model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var maintenance = new Maintenance
        {
            PropertyId = model.PropertyId,
            AccessToken = Guid.NewGuid(),
            MaintenanceProviderId = model.AssignedUserId,
        //    AssignedUserId = model.AssignedUserId,
            Title = model.Title,
            Description = model.Description,
            Priority = model.Priority,
            Status = model.Status,
            ScheduledDate = model.ScheduledDate
        };



        await _maintenanceService.CreateAsync(maintenance);

        TempData["Success"] = "Manutenção criada com sucesso.";

        var propriedade = await _propertyService.GetByIdAsync(model.PropertyId);

        var provider = await _maintenanceProviderService.GetByIdAsync(model.AssignedUserId);

        var link = Url.Action(
            "Task",
            "PublicMaintenance",
            new { token = maintenance.AccessToken },
            Request.Scheme);

        var body = $@"
<!DOCTYPE html>
<html>
<head>
    <meta charset='utf-8'>
</head>
<body style='font-family: Arial, Helvetica, sans-serif; background-color:#f4f4f4; padding:30px;'>

    <table width='100%' cellpadding='0' cellspacing='0'>
        <tr>
            <td align='center'>

                <table width='650' style='background:#fff;border-radius:8px;padding:30px;'>

                    <h2>CleanMadeira</h2>

                    <p>Olá <strong>{provider.Name}</strong>,</p>

                    <p>
                        Foi-lhe atribuído um novo pedido de manutenção.
                    </p>

                    <p><strong>Propriedade:</strong> {maintenance.Property.Name}</p>

                    <p><strong>Morada:</strong> {maintenance.Property.Address}</p>

                    <p><strong>Descrição:</strong></p>

                    <p>{maintenance.Description}</p>

                    <p><strong>Data:</strong> {maintenance.ScheduledDate:dd/MM/yyyy HH:mm}</p>

                    <br/>

                    <a href='{link}'
                       style='background:#0d6efd;
                              color:white;
                              padding:15px 30px;
                              text-decoration:none;
                              border-radius:5px;'>

                        Ver Pedido

                    </a>

                    <hr/>

                    <p style='font-size:12px;color:#777'>
                        Este email foi enviado automaticamente pelo CleanMadeira.
                    </p>

                </table>

            </td>
        </tr>
    </table>

</body>
</html>";

        await _emailService.SendEmailAsync(
              provider.Email!,
              "Nova manutenção atribuída",
               body);

        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Edit(Guid id)
    {

        var maintenance = await _maintenanceService.GetByIdAsync(id);

        if (maintenance == null)
            return NotFound();

        await LoadDropdowns();

        var vm = new EditMaintenanceVM
        {
            Id = maintenance.Id,
            PropriedadeId = maintenance.PropertyId,
            AssignedUserId = maintenance.AssignedUserId,
            Title = maintenance.Title,
            Description = maintenance.Description,
            Priority = maintenance.Priority,
            Status = maintenance.Status,
            ScheduledDate = maintenance.ScheduledDate
        };

        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(EditMaintenanceVM model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var maintenance = new Maintenance
        {
            Id = model.Id,
            PropertyId = model.PropriedadeId,
            AssignedUserId = model.AssignedUserId,
            Title = model.Title,
            Description = model.Description,
            Priority = model.Priority,
            Status = model.Status,
            ScheduledDate = model.ScheduledDate
        };

        await _maintenanceService.UpdateAsync(maintenance);


        TempData["Success"] = "Manutenção atualizada com sucesso.";

        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Delete(Guid id)
    {
        var maintenance = await _maintenanceService.GetByIdAsync(id);

        if (maintenance == null)
            return NotFound();

        var vm = new MaintenanceDetailsVM
        {
            Id = maintenance.Id,
            PropertyId = maintenance.PropertyId,
            AssignedUserId = maintenance.MaintenanceProviderId,
            PropertyName = maintenance.Property.Name,
            PropertyAddress = maintenance.Property.Address,
            AssignedUserName = maintenance.MaintenanceProvider.Name,
            Title = maintenance.Title,
            Description = maintenance.Description,
            Priority = maintenance.Priority,
            Status = maintenance.Status,
            ScheduledDate = maintenance.ScheduledDate,
            CreatedAt = maintenance.CreatedAt,
        };

        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(Guid id)
    {
        await _maintenanceService.DeleteAsync(id);

        TempData["Success"] = "Manutenção eliminada com sucesso.";

        return RedirectToAction(nameof(Index));
    }

    private async Task LoadDropdowns()
    {
        var user = await _userManager.GetUserAsync(User);

        var propriedades = await _propertyService.GetByUserAsync(user.Id);

        ViewBag.Propriedades = new SelectList(
            propriedades,
            "Id",
            "Name");

        var prestadores = await _maintenanceProviderService.GetByOwnerIdAsync(user.Id);

        ViewBag.Prestadores = new SelectList(
            prestadores,
            "Id",
            "Name",
            "Category");
    }

}