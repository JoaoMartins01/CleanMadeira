using CleanMadeira.Application.Interfaces.Services;
using CleanMadeira.Domain.Enums;
using CleanMadeira.Web.ViewModels.PublicMaintenance;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CleanMadeira.Web.Controllers
{
    [AllowAnonymous]
    public class PublicMaintenanceController : Controller
    {
        private readonly IMaintenanceService _maintenanceService;

        public PublicMaintenanceController(
            IMaintenanceService maintenanceService)
        {
            _maintenanceService = maintenanceService;
        }

        // GET: /PublicMaintenance/Task?token=...
        [HttpGet]
        public async Task<IActionResult> Task(Guid token)
        {
            if (token == Guid.Empty)
                return BadRequest("O token de acesso não é válido.");

            var maintenance =
                await _maintenanceService.GetByAccessTokenAsync(token);

            if (maintenance == null)
                return NotFound("O pedido de manutenção não foi encontrado.");

            var model = new PublicMaintenanceVM
            {
                AccessToken = maintenance.AccessToken,

                Title = maintenance.Title,

                Description = maintenance.Description,

                ScheduledDate = maintenance.ScheduledDate,

                Priority = maintenance.Priority,

                Status = maintenance.Status,

                ProviderName = maintenance.MaintenanceProvider?.Name
                    ?? "Prestador não definido",

                PropertyName = maintenance.Property?.Name
                    ?? "Propriedade não definida",

                PropertyAddress = maintenance.Property?.Address
            };

            return View(model);
        }

        // POST: /PublicMaintenance/Accept
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Accept(Guid token)
        {
            if (token == Guid.Empty)
                return BadRequest("O token de acesso não é válido.");

            var maintenance =
                await _maintenanceService.GetByAccessTokenAsync(token);

            if (maintenance == null)
                return NotFound();

            if (maintenance.Status != MaintenanceStatus.Pendente)
            {
                TempData["Error"] =
                    "Este pedido já foi respondido.";

                return RedirectToAction(
                    nameof(Task),
                    new { token });
            }

            maintenance.Status = MaintenanceStatus.Aceite;

            await _maintenanceService.UpdateAsync(maintenance);

            TempData["Success"] =
                "A tarefa de manutenção foi aceite.";

            return RedirectToAction(
                nameof(Task),
                new { token });
        }

        // POST: /PublicMaintenance/Reject
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Reject(Guid token, PublicMaintenanceVM model)
        {
            if (token == Guid.Empty)
                return BadRequest("O token de acesso não é válido.");

            var maintenance =
                await _maintenanceService.GetByAccessTokenAsync(token);

            if (maintenance == null)
                return NotFound();

            if (maintenance.Status != MaintenanceStatus.Pendente)
            {
                TempData["Error"] =
                    "Este pedido já foi respondido.";

                return RedirectToAction(
                    nameof(Task),
                    new { token });
            }

            maintenance.Status = MaintenanceStatus.Rejeitada;
            maintenance.RejectionReason = model.RejectionReason;


            await _maintenanceService.UpdateAsync(maintenance);

            TempData["Success"] =
                "A tarefa de manutenção foi recusada.";

            return RedirectToAction(
                nameof(Task),
                new { token });
        }

        // POST: /PublicMaintenance/Update
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(
            PublicMaintenanceVM model,
            string action)
        {
            if (model.AccessToken == Guid.Empty)
                return BadRequest("O token de acesso não é válido.");

            var maintenance =
                await _maintenanceService.GetByAccessTokenAsync(
                    model.AccessToken);

            if (maintenance == null)
                return NotFound();

            if (maintenance.Status == MaintenanceStatus.Rejeitada)
            {
                TempData["Error"] =
                    "Esta tarefa foi recusada e já não pode ser alterada.";

                return RedirectToAction(
                    nameof(Task),
                    new { token = model.AccessToken });
            }

            maintenance.ProviderNotes = model.ProviderNotes;

            switch (action?.ToLowerInvariant())
            {
                case "start":

                    if (maintenance.Status != MaintenanceStatus.Aceite)
                    {
                        TempData["Error"] =
                            "A tarefa tem de estar aceite antes de ser iniciada.";

                        return RedirectToAction(
                            nameof(Task),
                            new { token = model.AccessToken });
                    }

                    maintenance.Status =
                        MaintenanceStatus.EmProgresso;

                    TempData["Success"] =
                        "O trabalho foi iniciado.";

                    break;

                case "complete":

                    if (maintenance.Status !=
                        MaintenanceStatus.EmProgresso)
                    {
                        TempData["Error"] =
                            "A tarefa tem de estar em progresso antes de ser concluída.";

                        return RedirectToAction(
                            nameof(Task),
                            new { token = model.AccessToken });
                    }

                    maintenance.Status =
                        MaintenanceStatus.Completo;

                    TempData["Success"] =
                        "A tarefa foi marcada como concluída.";

                    break;

                default:

                    TempData["Error"] =
                        "A ação indicada não é válida.";

                    return RedirectToAction(
                        nameof(Task),
                        new { token = model.AccessToken });
            }

            await _maintenanceService.UpdateAsync(maintenance);

            return RedirectToAction(
                nameof(Task),
                new { token = model.AccessToken });
        }

        [HttpPost]
        public async Task<IActionResult> Start(Guid token)
        {
            var maintenance = await _maintenanceService.GetByAccessTokenAsync(token);

            if (maintenance == null)
                return NotFound();

            maintenance.Status = MaintenanceStatus.EmProgresso;

            await _maintenanceService.UpdateAsync(maintenance);

            return RedirectToAction(nameof(Task), new { token });
        }

        [HttpPost]
        public async Task<IActionResult> Complete(Guid token, PublicMaintenanceVM model)
        {
            var maintenance = await _maintenanceService.GetByAccessTokenAsync(token);

            if (maintenance == null)
                return NotFound();

            maintenance.Status = MaintenanceStatus.Completo;
            maintenance.ProviderNotes = model.ProviderNotes;

            await _maintenanceService.UpdateAsync(maintenance);

            return RedirectToAction(nameof(Task), new { token });
        }
    }
}