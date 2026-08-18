using CleanMadeira.Application.Contract;
using CleanMadeira.Application.Interfaces;
using CleanMadeira.Application.Services.Interface;
using CleanMadeira.Domain.Entities;
using CleanMadeira.Domain.Enums;
using CleanMadeira.Web.ViewModels.Maintenance;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mail;
using System.Security.Claims;

namespace CleanMadeira.Web.Controllers;

[Authorize]
public class MaintenanceReportController : Controller
{
    private readonly IMaintenanceReportService _maintenanceReportService;
    private readonly ICleaningTaskService _cleaningTaskService;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IEmailService _emailService;

    public MaintenanceReportController(
        IMaintenanceReportService maintenanceReportService,
        ICleaningTaskService cleaningTaskService,
        IEmailService emailService,
        UserManager<ApplicationUser> userManager)
    {
        _maintenanceReportService = maintenanceReportService;
        _cleaningTaskService = cleaningTaskService;
        _emailService = emailService;
        _userManager = userManager;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var user = await _userManager.GetUserAsync(User);

        var reports = await _maintenanceReportService
            .GetByOwnerIdAsync(user.Id);

        return View(reports);
    }

    [HttpGet]
    public async Task<IActionResult> ReportProblem(Guid cleaningTaskId)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var task = await _cleaningTaskService
            .GetByIdAndCleanerIdAsync(cleaningTaskId, userId);

        if (task == null)
            return NotFound();

        var model = new MaintenanceReportVM
        {
            CleaningTaskId = task.Id,
            PropertyId = task.PropertyId,
            PropertyName = task.Property.Name,
            PropertyAddress = task.Property.Address
        };

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ReportProblem(
        MaintenanceReportVM model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var task = await _cleaningTaskService
            .GetByIdAndCleanerIdAsync(model.CleaningTaskId, userId);

        if (task == null)
            return NotFound();

        var user = await _userManager.GetUserAsync(User);

        var report = new MaintenanceReport
        {
            Id = Guid.NewGuid(),
            CleaningTaskId = task.Id,
            PropertyId = task.PropertyId,
            ReportedByUserId = user.Id,
            Title = model.Title,
            Description = model.Description,
            Priority = model.Priority!.Value,
            ReportedAt = DateTime.UtcNow,
            Status = MaintenanceReportStatus.PendingReview,
        };

        await _maintenanceReportService.AddAsync(report);

        var reportUrl = Url.Action(
    "Details",
    "MaintenanceReport",
    new { id = report.Id },
    Request.Scheme);

        var localCreatedAt = report.ReportedAt.ToLocalTime();

        var body = $"""
<!DOCTYPE html>
<html lang="pt">
<head>
    <meta charset="UTF-8">
    <meta name="viewport"
          content="width=device-width, initial-scale=1.0">
    <title>Novo problema reportado</title>
</head>

<body style="
    margin: 0;
    padding: 0;
    background-color: #f4f6f8;
    font-family: Arial, Helvetica, sans-serif;
    color: #212529;">

    <table role="presentation"
           width="100%"
           cellspacing="0"
           cellpadding="0"
           border="0"
           style="background-color: #f4f6f8; padding: 32px 12px;">

        <tr>
            <td align="center">

                <table role="presentation"
                       width="100%"
                       cellspacing="0"
                       cellpadding="0"
                       border="0"
                       style="
                           max-width: 620px;
                           background-color: #ffffff;
                           border-radius: 14px;
                           overflow: hidden;
                           box-shadow: 0 4px 18px rgba(0,0,0,0.08);">

                    <tr>
                        <td style="
                            background-color: #0d6efd;
                            padding: 24px 30px;
                            text-align: center;">

                            <div style="
                                color: #ffffff;
                                font-size: 24px;
                                font-weight: 700;">
                                CleanMadeira
                            </div>

                            <div style="
                                color: #dbeafe;
                                font-size: 14px;
                                margin-top: 6px;">
                                Gestão operacional de alojamentos
                            </div>

                        </td>
                    </tr>

                    <tr>
                        <td style="padding: 32px 30px;">

                            <h1 style="
                                margin: 0 0 14px;
                                font-size: 24px;
                                color: #212529;">
                                Novo problema reportado
                            </h1>

                            <p style="
                                margin: 0 0 24px;
                                color: #6c757d;
                                font-size: 16px;
                                line-height: 1.6;">
                                Foi identificado um problema durante uma tarefa
                                de limpeza. O reporte encontra-se pendente de
                                análise pelo gestor.
                            </p>

                            <table role="presentation"
                                   width="100%"
                                   cellspacing="0"
                                   cellpadding="0"
                                   border="0"
                                   style="
                                       background-color: #f8f9fa;
                                       border: 1px solid #e9ecef;
                                       border-radius: 10px;
                                       margin-bottom: 24px;">

                                <tr>
                                    <td style="padding: 20px;">

                                        <p style="margin: 0 0 14px;">
                                            <strong>Propriedade:</strong><br>
                                            <span style="color: #495057;">
                                                {report.Property?.Name ?? "Não disponível"}
                                            </span>
                                        </p>

                                        <p style="margin: 0 0 14px;">
                                            <strong>Problema:</strong><br>
                                            <span style="color: #495057;">
                                                {report.Title}
                                            </span>
                                        </p>

                                        <p style="margin: 0 0 14px;">
                                            <strong>Prioridade:</strong><br>
                                            <span style="color: #495057;">
                                                {report.Priority}
                                            </span>
                                        </p>

                                        <p style="margin: 0 0 14px;">
                                            <strong>Reportado por:</strong><br>
                                            <span style="color: #495057;">
                                                {report.ReportedByUser?.PrimeiroNome}
                                                {report.ReportedByUser?.UltimoNome}
                                            </span>
                                        </p>

                                        <p style="margin: 0;">
                                            <strong>Data do reporte:</strong><br>
                                            <span style="color: #495057;">
                                                {localCreatedAt:dd/MM/yyyy 'às' HH:mm}
                                            </span>
                                        </p>

                                    </td>
                                </tr>
                            </table>

                            <div style="
                                border-left: 4px solid #ffc107;
                                background-color: #fff8e1;
                                padding: 16px;
                                margin-bottom: 26px;
                                border-radius: 6px;">

                                <strong>Descrição</strong>

                                <p style="
                                    margin: 8px 0 0;
                                    color: #495057;
                                    line-height: 1.6;
                                    white-space: pre-line;">
                                    {report.Description}
                                </p>

                            </div>

                            <table role="presentation"
                                   cellspacing="0"
                                   cellpadding="0"
                                   border="0"
                                   align="center">

                                <tr>
                                    <td align="center"
                                        bgcolor="#0d6efd"
                                        style="border-radius: 8px;">

                                        <a href="{reportUrl}"
                                           style="
                                               display: inline-block;
                                               padding: 14px 24px;
                                               color: #ffffff;
                                               text-decoration: none;
                                               font-size: 16px;
                                               font-weight: 600;">
                                            Analisar reporte
                                        </a>

                                    </td>
                                </tr>

                            </table>

                            <p style="
                                margin: 26px 0 0;
                                color: #6c757d;
                                font-size: 14px;
                                line-height: 1.5;">
                                Depois de analisar o reporte, poderá convertê-lo
                                numa manutenção, marcá-lo como resolvido sem
                                intervenção ou rejeitá-lo.
                            </p>

                        </td>
                    </tr>

                    <tr>
                        <td style="
                            padding: 20px 30px;
                            background-color: #f8f9fa;
                            text-align: center;
                            color: #868e96;
                            font-size: 12px;">

                            Esta é uma mensagem automática do CleanMadeira.
                            Não responda diretamente a este email.

                        </td>
                    </tr>

                </table>

            </td>
        </tr>

    </table>

</body>
</html>
""";

        await _emailService.SendEmailAsync(
    report.Property.ApplicationUser?.Email,
    "Novo problema reportado",
    body);


        TempData["Success"] =
            "O problema foi reportado com sucesso.";

        return RedirectToAction(
            "Details",
            "CleaningTask",
            new { id = task.Id });
    }

    [HttpGet]
    public async Task<IActionResult> Details(Guid id)
    {
        var report = await _maintenanceReportService
            .GetByIdAsync(id);

        if (report == null)
            return NotFound();

        return View(report);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Reject(Guid id)
    {
        await _maintenanceReportService
            .MarkAsRejectedAsync(id);

        TempData["Success"] =
            "O reporte foi rejeitado.";

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Resolve(Guid id)
    {
        await _maintenanceReportService
            .MarkAsResolvedWithoutMaintenanceAsync(id);

        TempData["Success"] =
            "O reporte foi marcado como resolvido.";

        return RedirectToAction(nameof(Index));
    }
}