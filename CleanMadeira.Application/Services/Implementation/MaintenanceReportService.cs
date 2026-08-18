using CleanMadeira.Application.Interfaces;
using CleanMadeira.Domain.Entities;
using CleanMadeira.Domain.Enums;
using CleanMadeira.Domain.Interfaces;

namespace CleanMadeira.Application.Services;

public class MaintenanceReportService : IMaintenanceReportService
{
    private readonly IMaintenanceReportRepository
        _maintenanceReportRepository;

    public MaintenanceReportService(
        IMaintenanceReportRepository maintenanceReportRepository)
    {
        _maintenanceReportRepository = maintenanceReportRepository;
    }

    public async Task<IEnumerable<MaintenanceReport>> GetAllAsync()
    {
        return await _maintenanceReportRepository.GetAllAsync();
    }

    public async Task<IEnumerable<MaintenanceReport>> GetByOwnerIdAsync(
        Guid ownerId)
    {
        return await _maintenanceReportRepository
            .GetByOwnerIdAsync(ownerId);
    }

    public async Task<IEnumerable<MaintenanceReport>>
        GetPendingByOwnerIdAsync(Guid ownerId)
    {
        var reports = await _maintenanceReportRepository
            .GetByOwnerIdAsync(ownerId);

        return reports.Where(report =>
            report.Status ==
            MaintenanceReportStatus.PendingReview);
    }

    public async Task<MaintenanceReport?> GetByIdAsync(Guid id)
    {
        return await _maintenanceReportRepository.GetByIdAsync(id);
    }

    public async Task AddAsync(MaintenanceReport report)
    {
        if (report == null)
            throw new ArgumentNullException(nameof(report));

        if (string.IsNullOrWhiteSpace(report.Title))
        {
            throw new ArgumentException(
                "O título do reporte é obrigatório.",
                nameof(report));
        }

        if (string.IsNullOrWhiteSpace(report.Description))
        {
            throw new ArgumentException(
                "A descrição do reporte é obrigatória.",
                nameof(report));
        }

        if (report.Id == Guid.Empty)
            report.Id = Guid.NewGuid();

        report.Status = MaintenanceReportStatus.PendingReview;

        await _maintenanceReportRepository.AddAsync(report);
    }

    public async Task UpdateAsync(MaintenanceReport report)
    {
        if (report == null)
            throw new ArgumentNullException(nameof(report));

        var existingReport =
            await _maintenanceReportRepository.GetByIdAsync(report.Id);

        if (existingReport == null)
        {
            throw new KeyNotFoundException(
                "O reporte de manutenção não foi encontrado.");
        }

        existingReport.Title = report.Title;
        existingReport.Description = report.Description;
        existingReport.Priority = report.Priority;

        await _maintenanceReportRepository.UpdateAsync(existingReport);
    }

    public async Task DeleteAsync(MaintenanceReport report)
    {
        if (report == null)
            throw new ArgumentNullException(nameof(report));

        await _maintenanceReportRepository.DeleteAsync(report);
    }

    public async Task MarkAsConvertedAsync(
        Guid reportId,
        Guid maintenanceId)
    {
        var report = await GetRequiredReportAsync(reportId);

        if (maintenanceId == Guid.Empty)
        {
            throw new ArgumentException(
                "O identificador da manutenção é inválido.",
                nameof(maintenanceId));
        }

        EnsurePending(report);

        report.MaintenanceId = maintenanceId;
        report.Status =
            MaintenanceReportStatus.ConvertedToMaintenance;

        await _maintenanceReportRepository.UpdateAsync(report);
    }

    public async Task MarkAsRejectedAsync(Guid reportId)
    {
        var report = await GetRequiredReportAsync(reportId);

        EnsurePending(report);

        report.Status = MaintenanceReportStatus.Rejected;

        await _maintenanceReportRepository.UpdateAsync(report);
    }

    public async Task MarkAsResolvedWithoutMaintenanceAsync(
        Guid reportId)
    {
        var report = await GetRequiredReportAsync(reportId);

        EnsurePending(report);

        report.Status =
            MaintenanceReportStatus.ResolvedWithoutMaintenance;

        await _maintenanceReportRepository.UpdateAsync(report);
    }

    private async Task<MaintenanceReport> GetRequiredReportAsync(
        Guid reportId)
    {
        var report =
            await _maintenanceReportRepository.GetByIdAsync(reportId);

        if (report == null)
        {
            throw new KeyNotFoundException(
                "O reporte de manutenção não foi encontrado.");
        }

        return report;
    }

    private static void EnsurePending(MaintenanceReport report)
    {
        if (report.Status !=
            MaintenanceReportStatus.PendingReview)
        {
            throw new InvalidOperationException(
                "Este reporte já foi analisado.");
        }
    }
}
