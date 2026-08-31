using CleanMadeira.Application.Interfaces.Services;
using CleanMadeira.Application.Services.Interface;
using CleanMadeira.Domain.Entities;
using CleanMadeira.Web.ViewModels.CleaningTask;
using CleanMadeira.Web.ViewModels.Report;
using ClosedXML.Excel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using QuestPDF.Fluent;

[Authorize]
public class ReportController : Controller
{
    private readonly ICleaningTaskService _cleaningTaskService;
    private readonly IInventoryService _inventoryService;
    private readonly IMaintenanceService _maintenanceService;
    private readonly UserManager<ApplicationUser> _userManager;

    public ReportController(
        ICleaningTaskService cleaningTaskService,
        IInventoryService inventoryService,
        IMaintenanceService maintenanceService,
        UserManager<ApplicationUser> userManager)
    {
        _cleaningTaskService = cleaningTaskService;
        _inventoryService = inventoryService;
        _maintenanceService = maintenanceService;
        _userManager = userManager;
    }


    [HttpGet]
    public IActionResult Index()
    {
        return View();
    }


    [HttpGet]
    public async Task<IActionResult> MonthlyCleanings(
        int? year,
        int? month)
    {
        var now = DateTime.Now;

        var selectedYear = year ?? now.Year;
        var selectedMonth = month ?? now.Month;

        if (selectedMonth < 1 || selectedMonth > 12)
        {
            selectedMonth = now.Month;
        }

        var userIdString = _userManager.GetUserId(User);

        if (!Guid.TryParse(userIdString, out var userId))
        {
            return Unauthorized();
        }

        var report = await _cleaningTaskService
            .GetMonthlyReportAsync(
                userId,
                selectedYear,
                selectedMonth);

        var model = new MonthlyCleaningReportVM
        {
            Year = report.Year,
            Month = report.Month,

            Total = report.Total,
            Completed = report.Completed,
            Pending = report.Pending,
            InProgress = report.InProgress,
            Cancelled = report.Cancelled,

            Tasks = report.Tasks
                .Select(x => new MonthlyCleaningReportItemVM
                {
                    Id = x.Id,
                    PropertyName = x.PropertyName,
                    CleanerName = x.CleanerName,
                    ScheduledDate = x.ScheduledDate,
                    Status = x.Status,
                    Priority = x.Priority
                })
                .ToList()
        };

        return View(model);
    }

    [HttpGet]
    public async Task<IActionResult> ExportMonthlyCleaningsExcel(
    int year,
    int month)
    {
        var userIdString = _userManager.GetUserId(User);

        if (!Guid.TryParse(userIdString, out var userId))
            return Unauthorized();

        var report = await _cleaningTaskService
            .GetMonthlyReportAsync(userId, year, month);

        using var workbook = new XLWorkbook();

        var worksheet = workbook.Worksheets.Add("Limpezas");

        worksheet.Cell(1, 1).Value = "Relatório Mensal de Limpezas";
        worksheet.Cell(2, 1).Value = $"Mês: {month}/{year}";

        worksheet.Cell(4, 1).Value = "Total";
        worksheet.Cell(4, 2).Value = report.Total;

        worksheet.Cell(5, 1).Value = "Concluídas";
        worksheet.Cell(5, 2).Value = report.Completed;

        worksheet.Cell(6, 1).Value = "Pendentes";
        worksheet.Cell(6, 2).Value = report.Pending;

        worksheet.Cell(7, 1).Value = "Em progresso";
        worksheet.Cell(7, 2).Value = report.InProgress;

        worksheet.Cell(8, 1).Value = "Canceladas";
        worksheet.Cell(8, 2).Value = report.Cancelled;


        var row = 11;

        worksheet.Cell(row, 1).Value = "Data";
        worksheet.Cell(row, 2).Value = "Hora";
        worksheet.Cell(row, 3).Value = "Propriedade";
        worksheet.Cell(row, 4).Value = "Limpador";
        worksheet.Cell(row, 5).Value = "Estado";
        worksheet.Cell(row, 6).Value = "Prioridade";

        row++;

        foreach (var task in report.Tasks)
        {
            worksheet.Cell(row, 1).Value =
                task.ScheduledDate.ToString("dd/MM/yyyy");

            worksheet.Cell(row, 2).Value =
                task.ScheduledDate.ToString("HH:mm");

            worksheet.Cell(row, 3).Value =
                task.PropertyName;

            worksheet.Cell(row, 4).Value =
                task.CleanerName ?? "Não atribuído";

            worksheet.Cell(row, 5).Value =
                task.Status;

            worksheet.Cell(row, 6).Value =
                task.Priority;

            row++;
        }

        worksheet.Columns().AdjustToContents();

        using var stream = new MemoryStream();

        workbook.SaveAs(stream);

        return File(
            stream.ToArray(),
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            $"limpezas-{year}-{month:D2}.xlsx"
        );
    }

    [HttpGet]
    public async Task<IActionResult> ExportMonthlyCleaningsPdf(
    int year,
    int month)
    {
        var userIdString = _userManager.GetUserId(User);

        if (!Guid.TryParse(userIdString, out var userId))
            return Unauthorized();

        var report = await _cleaningTaskService
            .GetMonthlyReportAsync(userId, year, month);

        var pdf = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Margin(30);

                page.Header()
                    .Text("CleanMadeira - Relatório Mensal de Limpezas")
                    .FontSize(18)
                    .Bold();

                page.Content()
                    .PaddingVertical(20)
                    .Column(column =>
                    {
                        column.Spacing(12);

                        column.Item()
                            .Text($"Período: {month:D2}/{year}");

                        column.Item().Row(row =>
                        {
                            row.RelativeItem()
                                .Text($"Total: {report.Total}");

                            row.RelativeItem()
                                .Text($"Concluídas: {report.Completed}");

                            row.RelativeItem()
                                .Text($"Pendentes: {report.Pending}");
                        });

                        column.Item().Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.RelativeColumn();
                                columns.RelativeColumn();
                                columns.RelativeColumn(2);
                                columns.RelativeColumn(2);
                                columns.RelativeColumn();
                                columns.RelativeColumn();
                            });

                            table.Header(header =>
                            {
                                header.Cell().Text("Data").Bold();
                                header.Cell().Text("Hora").Bold();
                                header.Cell().Text("Propriedade").Bold();
                                header.Cell().Text("Limpador").Bold();
                                header.Cell().Text("Estado").Bold();
                                header.Cell().Text("Prioridade").Bold();
                            });

                            foreach (var task in report.Tasks)
                            {
                                table.Cell()
                                    .Text(task.ScheduledDate.ToString("dd/MM/yyyy"));

                                table.Cell()
                                    .Text(task.ScheduledDate.ToString("HH:mm"));

                                table.Cell()
                                    .Text(task.PropertyName);

                                table.Cell()
                                    .Text(task.CleanerName ?? "Não atribuído");

                                table.Cell()
                                    .Text(task.Status);

                                table.Cell()
                                    .Text(task.Priority);
                            }
                        });
                    });

                page.Footer()
                    .AlignCenter()
                    .Text(text =>
                    {
                        text.Span("Página ");
                        text.CurrentPageNumber();
                    });
            });
        });

        var bytes = pdf.GeneratePdf();

        return File(
            bytes,
            "application/pdf",
            $"limpezas-{year}-{month:D2}.pdf"
        );
    }

    [HttpGet]
    public async Task<IActionResult> MonthlyMaintenances(
    int? year,
    int? month)
    {
        var now = DateTime.Now;

        var selectedYear = year ?? now.Year;
        var selectedMonth = month ?? now.Month;

        if (selectedMonth < 1 || selectedMonth > 12)
        {
            selectedMonth = now.Month;
        }

        var userIdString = _userManager.GetUserId(User);

        if (!Guid.TryParse(userIdString, out var userId))
        {
            return Unauthorized();
        }

        var report = await _maintenanceService
            .GetMonthlyReportAsync(
                userId,
                selectedYear,
                selectedMonth);

        var model = new MonthlyMaintenanceReportVM
        {
            Year = report.Year,
            Month = report.Month,

            Total = report.Total,
            Pending = report.Pending,
            Accepted = report.Accepted,
            InProgress = report.InProgress,
            Completed = report.Completed,
            Cancelled = report.Cancelled,
            Rejected = report.Rejected,

            Maintenances = report.Maintenances
                .Select(x => new MonthlyMaintenanceReportItemVM
                {
                    Id = x.Id,
                    Title = x.Title,
                    PropertyName = x.PropertyName,
                    ProviderName = x.ProviderName,
                    ScheduledDate = x.ScheduledDate,
                    Status = x.Status,
                    Priority = x.Priority
                })
                .ToList()
        };

        return View(model);
    }

    [HttpGet]
    public async Task<IActionResult> ExportMonthlyMaintenancesExcel(
    int year,
    int month)
    {
        var userIdString = _userManager.GetUserId(User);

        if (!Guid.TryParse(userIdString, out var userId))
            return Unauthorized();

        var report = await _maintenanceService
            .GetMonthlyReportAsync(
                userId,
                year,
                month);

        using var workbook = new XLWorkbook();

        var worksheet =
            workbook.Worksheets.Add("Manutenções");

        worksheet.Cell(1, 1).Value =
            "Relatório Mensal de Manutenções";

        worksheet.Cell(2, 1).Value =
            $"Período: {month:D2}/{year}";


        // RESUMO

        worksheet.Cell(4, 1).Value = "Total";
        worksheet.Cell(4, 2).Value = report.Total;

        worksheet.Cell(5, 1).Value = "Pendentes";
        worksheet.Cell(5, 2).Value = report.Pending;

        worksheet.Cell(6, 1).Value = "Aceites";
        worksheet.Cell(6, 2).Value = report.Accepted;

        worksheet.Cell(7, 1).Value = "Em progresso";
        worksheet.Cell(7, 2).Value = report.InProgress;

        worksheet.Cell(8, 1).Value = "Concluídas";
        worksheet.Cell(8, 2).Value = report.Completed;

        worksheet.Cell(9, 1).Value = "Canceladas";
        worksheet.Cell(9, 2).Value = report.Cancelled;

        worksheet.Cell(10, 1).Value = "Rejeitadas";
        worksheet.Cell(10, 2).Value = report.Rejected;


        // TABELA

        var row = 13;

        worksheet.Cell(row, 1).Value = "Data";
        worksheet.Cell(row, 2).Value = "Hora";
        worksheet.Cell(row, 3).Value = "Título";
        worksheet.Cell(row, 4).Value = "Propriedade";
        worksheet.Cell(row, 5).Value = "Prestador";
        worksheet.Cell(row, 6).Value = "Estado";
        worksheet.Cell(row, 7).Value = "Prioridade";

        // Cabeçalho a bold
        worksheet.Range(row, 1, row, 7)
            .Style.Font.Bold = true;

        row++;

        foreach (var maintenance in report.Maintenances)
        {
            worksheet.Cell(row, 1).Value =
                maintenance.ScheduledDate
                    .ToString("dd/MM/yyyy");

            worksheet.Cell(row, 2).Value =
                maintenance.ScheduledDate
                    .ToString("HH:mm");

            worksheet.Cell(row, 3).Value =
                maintenance.Title;

            worksheet.Cell(row, 4).Value =
                maintenance.PropertyName;

            worksheet.Cell(row, 5).Value =
                maintenance.ProviderName
                ?? "Não atribuído";

            worksheet.Cell(row, 6).Value =
                maintenance.Status;

            worksheet.Cell(row, 7).Value =
                maintenance.Priority;

            row++;
        }

        worksheet.Columns().AdjustToContents();

        using var stream = new MemoryStream();

        workbook.SaveAs(stream);

        return File(
            stream.ToArray(),
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            $"manutencoes-{year}-{month:D2}.xlsx"
        );
    }

    [HttpGet]
    public async Task<IActionResult> ExportMonthlyMaintenancesPdf(
    int year,
    int month)
    {
        var userIdString = _userManager.GetUserId(User);

        if (!Guid.TryParse(userIdString, out var userId))
            return Unauthorized();

        var report = await _maintenanceService
            .GetMonthlyReportAsync(
                userId,
                year,
                month);

        var document = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Margin(30);

                page.Header()
                    .Column(column =>
                    {
                        column.Item()
                            .Text("CleanMadeira")
                            .FontSize(12);

                        column.Item()
                            .Text("Relatório Mensal de Manutenções")
                            .FontSize(20)
                            .Bold();

                        column.Item()
                            .Text($"Período: {month:D2}/{year}")
                            .FontSize(11);
                    });


                page.Content()
                    .PaddingVertical(20)
                    .Column(column =>
                    {
                        column.Spacing(16);


                        // RESUMO

                        column.Item()
                            .Row(row =>
                            {
                                row.RelativeItem()
                                    .Text($"Total: {report.Total}");

                                row.RelativeItem()
                                    .Text($"Pendentes: {report.Pending}");

                                row.RelativeItem()
                                    .Text($"Aceites: {report.Accepted}");
                            });


                        column.Item()
                            .Row(row =>
                            {
                                row.RelativeItem()
                                    .Text($"Em progresso: {report.InProgress}");

                                row.RelativeItem()
                                    .Text($"Concluídas: {report.Completed}");

                                row.RelativeItem()
                                    .Text($"Rejeitadas: {report.Rejected}");
                            });


                        // TABELA

                        column.Item()
                            .Table(table =>
                            {
                                table.ColumnsDefinition(columns =>
                                {
                                    columns.RelativeColumn(1.3f);
                                    columns.RelativeColumn(2f);
                                    columns.RelativeColumn(2f);
                                    columns.RelativeColumn(2f);
                                    columns.RelativeColumn(1.4f);
                                    columns.RelativeColumn(1.4f);
                                });


                                table.Header(header =>
                                {
                                    header.Cell()
                                        .Text("Data")
                                        .Bold();

                                    header.Cell()
                                        .Text("Título")
                                        .Bold();

                                    header.Cell()
                                        .Text("Propriedade")
                                        .Bold();

                                    header.Cell()
                                        .Text("Prestador")
                                        .Bold();

                                    header.Cell()
                                        .Text("Estado")
                                        .Bold();

                                    header.Cell()
                                        .Text("Prioridade")
                                        .Bold();
                                });


                                foreach (var maintenance
                                         in report.Maintenances)
                                {
                                    table.Cell()
                                        .Text(
                                            maintenance.ScheduledDate
                                                .ToString(
                                                    "dd/MM/yyyy HH:mm"));

                                    table.Cell()
                                        .Text(maintenance.Title);

                                    table.Cell()
                                        .Text(
                                            maintenance.PropertyName);

                                    table.Cell()
                                        .Text(
                                            maintenance.ProviderName
                                            ?? "Não atribuído");

                                    table.Cell()
                                        .Text(
                                            maintenance.Status);

                                    table.Cell()
                                        .Text(
                                            maintenance.Priority);
                                }
                            });
                    });


                page.Footer()
                    .AlignCenter()
                    .Text(text =>
                    {
                        text.Span("Página ");
                        text.CurrentPageNumber();
                    });
            });
        });

        var pdfBytes =
            document.GeneratePdf();

        return File(
            pdfBytes,
            "application/pdf",
            $"manutencoes-{year}-{month:D2}.pdf"
        );
    }

    [HttpGet]
    public async Task<IActionResult> Inventory()
    {
        var userIdString =
            _userManager.GetUserId(User);

        if (!Guid.TryParse(userIdString, out var userId))
        {
            return Unauthorized();
        }

        var report = await _inventoryService
            .GetInventoryReportAsync(userId);

        var model = new InventoryReportVM
        {
            TotalItems =
                report.TotalItems,

            LowStockItems =
                report.LowStockItems,

            OutOfStockItems =
                report.OutOfStockItems,

            PropertiesWithInventory =
                report.PropertiesWithInventory,

            Items = report.Items
                .Select(x => new InventoryReportItemVM
                {
                    Id = x.Id,

                    Name = x.Name,

                    PropertyName =
                        x.PropertyName,

                    Quantity =
                        x.Quantity,

                    MinimumQuantity =
                        x.MinimumQuantity,

                    Unit =
                        x.Unit,

                    IsLowStock =
                        x.IsLowStock,

                    IsOutOfStock =
                        x.IsOutOfStock
                })
                .ToList()
        };

        return View(model);
    }

    [HttpGet]
    public async Task<IActionResult> ExportInventoryExcel()
    {
        var userIdString = _userManager.GetUserId(User);

        if (!Guid.TryParse(userIdString, out var userId))
            return Unauthorized();

        var report = await _inventoryService
            .GetInventoryReportAsync(userId);

        using var workbook = new XLWorkbook();

        var worksheet =
            workbook.Worksheets.Add("Inventário");

        worksheet.Cell(1, 1).Value =
            "Relatório de Inventário";

        // Resumo
        worksheet.Cell(3, 1).Value = "Produtos";
        worksheet.Cell(3, 2).Value = report.TotalItems;

        worksheet.Cell(4, 1).Value = "Stock baixo";
        worksheet.Cell(4, 2).Value = report.LowStockItems;

        worksheet.Cell(5, 1).Value = "Sem stock";
        worksheet.Cell(5, 2).Value = report.OutOfStockItems;

        worksheet.Cell(6, 1).Value = "Propriedades";
        worksheet.Cell(6, 2).Value = report.PropertiesWithInventory;


        // Cabeçalho
        var row = 9;

        worksheet.Cell(row, 1).Value = "Produto";
        worksheet.Cell(row, 2).Value = "Propriedade";
        worksheet.Cell(row, 3).Value = "Quantidade";
        worksheet.Cell(row, 4).Value = "Unidade";
        worksheet.Cell(row, 5).Value = "Stock mínimo";
        worksheet.Cell(row, 6).Value = "Estado";

        worksheet.Range(row, 1, row, 6)
            .Style.Font.Bold = true;

        row++;

        foreach (var item in report.Items)
        {
            var status =
                item.IsOutOfStock
                    ? "Sem stock"
                    : item.IsLowStock
                        ? "Stock baixo"
                        : "Disponível";

            worksheet.Cell(row, 1).Value =
                item.Name;

            worksheet.Cell(row, 2).Value =
                item.PropertyName;

            worksheet.Cell(row, 3).Value =
                item.Quantity;

            worksheet.Cell(row, 4).Value =
                item.Unit ?? "";

            worksheet.Cell(row, 5).Value =
                item.MinimumQuantity;

            worksheet.Cell(row, 6).Value =
                status;

            row++;
        }

        worksheet.Columns()
            .AdjustToContents();

        using var stream =
            new MemoryStream();

        workbook.SaveAs(stream);

        return File(
            stream.ToArray(),
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            $"inventario-{DateTime.Now:yyyy-MM-dd}.xlsx"
        );
    }

    [HttpGet]
    public async Task<IActionResult> ExportInventoryPdf()
    {
        var userIdString =
            _userManager.GetUserId(User);

        if (!Guid.TryParse(
            userIdString,
            out var userId))
        {
            return Unauthorized();
        }

        var report =
            await _inventoryService
                .GetInventoryReportAsync(userId);

        var document =
            Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Margin(30);

                    page.Header()
                        .Column(column =>
                        {
                            column.Item()
                                .Text("CleanMadeira")
                                .FontSize(12);

                            column.Item()
                                .Text("Relatório de Inventário")
                                .FontSize(20)
                                .Bold();

                            column.Item()
                                .Text(
                                    $"Gerado em {DateTime.Now:dd/MM/yyyy HH:mm}")
                                .FontSize(10);
                        });


                    page.Content()
                        .PaddingVertical(20)
                        .Column(column =>
                        {
                            column.Spacing(18);


                            // RESUMO
                            column.Item()
                                .Row(row =>
                                {
                                    row.RelativeItem()
                                        .Text(
                                            $"Produtos: {report.TotalItems}");

                                    row.RelativeItem()
                                        .Text(
                                            $"Stock baixo: {report.LowStockItems}");

                                    row.RelativeItem()
                                        .Text(
                                            $"Sem stock: {report.OutOfStockItems}");

                                    row.RelativeItem()
                                        .Text(
                                            $"Propriedades: {report.PropertiesWithInventory}");
                                });


                            // TABELA
                            column.Item()
                                .Table(table =>
                                {
                                    table.ColumnsDefinition(
                                        columns =>
                                        {
                                            columns.RelativeColumn(2);
                                            columns.RelativeColumn(2);
                                            columns.RelativeColumn(1);
                                            columns.RelativeColumn(1);
                                            columns.RelativeColumn(1);
                                            columns.RelativeColumn(1.5f);
                                        });


                                    table.Header(header =>
                                    {
                                        header.Cell()
                                            .Text("Produto")
                                            .Bold();

                                        header.Cell()
                                            .Text("Propriedade")
                                            .Bold();

                                        header.Cell()
                                            .Text("Qtd.")
                                            .Bold();

                                        header.Cell()
                                            .Text("Unidade")
                                            .Bold();

                                        header.Cell()
                                            .Text("Mínimo")
                                            .Bold();

                                        header.Cell()
                                            .Text("Estado")
                                            .Bold();
                                    });


                                    foreach (var item
                                             in report.Items)
                                    {
                                        var status =
                                            item.IsOutOfStock
                                                ? "Sem stock"
                                                : item.IsLowStock
                                                    ? "Stock baixo"
                                                    : "Disponível";

                                        table.Cell()
                                            .Text(item.Name);

                                        table.Cell()
                                            .Text(item.PropertyName);

                                        table.Cell()
                                            .Text(
                                                item.Quantity.ToString());

                                        table.Cell()
                                            .Text(
                                                item.Unit ?? "");

                                        table.Cell()
                                            .Text(
                                                item.MinimumQuantity
                                                    .ToString());

                                        table.Cell()
                                            .Text(status);
                                    }
                                });
                        });


                    page.Footer()
                        .AlignCenter()
                        .Text(text =>
                        {
                            text.Span("Página ");
                            text.CurrentPageNumber();
                        });
                });
            });

        var pdfBytes =
            document.GeneratePdf();

        return File(
            pdfBytes,
            "application/pdf",
            $"inventario-{DateTime.Now:yyyy-MM-dd}.pdf"
        );
    }

}
