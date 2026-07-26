using CleanMadeira.Application.Services.Interface;
using CleanMadeira.Domain.Entities;
using CleanMadeira.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CleanMadeira.Web.Controllers;

[Authorize]
public class CalendarIntegrationController : Controller
{
    private readonly ICalendarIntegrationService _calendarIntegrationService;
    private readonly ICalendarSyncService _calendarSyncService;
    private readonly IPropertyService _propertyService;

    public CalendarIntegrationController(
        ICalendarIntegrationService calendarIntegrationService,
        ICalendarSyncService calendarSyncService,
        IPropertyService propertyService)
    {
        _calendarIntegrationService = calendarIntegrationService;
        _calendarSyncService = calendarSyncService;
        _propertyService = propertyService;
    }

    [HttpGet]
    public async Task<IActionResult> Index(Guid propriedadeId)
    {
        var userId = GetCurrentUserId();

        var propriedade =
            await _propertyService.GetByIdAsync(propriedadeId);

        if (propriedade is null)
            return NotFound();

        if (propriedade.ApplicationUserId != userId)
            return Forbid();

        var integrations =
            await _calendarIntegrationService.GetByPropertyIdAsync(
                propriedadeId,
                userId);

        var model = new CalendarIntegrationsPageVM
        {
            PropriedadeId = propriedade.Id,

            PropriedadeNome = propriedade.Name,

            NovaIntegracao = new CalendarIntegrationVM
            {
                PropriedadeId = propriedade.Id,
                IsEnabled = true
            },

            Integracoes = integrations
                .Select(x => new CalendarIntegrationVM
                {
                    Id = x.Id,
                    PropriedadeId = x.PropertyId,
                    Provider = x.Provider,
                    CalendarUrl = x.CalendarUrl,
                    IsEnabled = x.IsEnabled,
                    LastSync = x.LastSync
                })
                .ToList()
        };

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(
        CalendarIntegrationsPageVM model)
    {
        model.NovaIntegracao.PropriedadeId =
            model.PropriedadeId;

        if (!ModelState.IsValid)
        {
            await ReloadPageAsync(model);
            return View("Index", model);
        }

        var entity = new CalendarIntegration
        {
            PropertyId =
                model.NovaIntegracao.PropriedadeId,

            Provider =
                model.NovaIntegracao.Provider,

            CalendarUrl =
                model.NovaIntegracao.CalendarUrl,

            IsEnabled =
                model.NovaIntegracao.IsEnabled
        };

        var result = await _calendarIntegrationService.CreateAsync(
            entity,
            GetCurrentUserId());

        TempData[result.Success ? "Success" : "Error"] =
            result.Message;

        return RedirectToAction(
            nameof(Index),
            new
            {
                propriedadeId = model.PropriedadeId
            });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(
        Guid id,
        Guid propriedadeId)
    {
        var result = await _calendarIntegrationService.DeleteAsync(
            id,
            GetCurrentUserId());

        TempData[result.Success ? "Success" : "Error"] =
            result.Message;

        return RedirectToAction(
            nameof(Index),
            new { propriedadeId });
    }

    private async Task ReloadPageAsync(
        CalendarIntegrationsPageVM model)
    {
        var userId = GetCurrentUserId();

        var property =
            await _propertyService.GetByIdAsync(
                model.PropriedadeId);

        if (property is not null)
        {
            model.PropriedadeNome = property.Name;
        }

        var integrations =
            await _calendarIntegrationService.GetByPropertyIdAsync(
                model.PropriedadeId,
                userId);

        model.Integracoes = integrations
            .Select(x => new CalendarIntegrationVM
            {
                Id = x.Id,
                PropriedadeId = x.PropertyId,
                Provider = x.Provider,
                CalendarUrl = x.CalendarUrl,
                IsEnabled = x.IsEnabled,
                LastSync = x.LastSync
            })
            .ToList();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Sync(
    Guid id,
    Guid propriedadeId,
    CancellationToken cancellationToken)
    {
        var integration = await _calendarIntegrationService
            .GetByIdAsync(id);

        if (integration == null)
        {
            TempData["Error"] =
                "A integração de calendário não foi encontrada.";

            return RedirectToAction(
                "Details",
                "Propriedade",
                new { id = propriedadeId });
        }

        if (integration.PropertyId != propriedadeId)
        {
            return BadRequest();
        }

        var result = await _calendarSyncService.SyncAsync(
            id,
            cancellationToken);

        if (result.Success)
        {
            TempData["Success"] = result.Message;
        }
        else
        {
            TempData["Error"] = result.Message;
        }

        return RedirectToAction(
            "Details",
            "Propriedade",
            new { id = propriedadeId });
    }

    private Guid GetCurrentUserId()
    {
        var value =
            User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (!Guid.TryParse(value, out var userId))
            throw new UnauthorizedAccessException();

        return userId;
    }
}