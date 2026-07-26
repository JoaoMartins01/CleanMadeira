using CleanMadeira.Domain.Entities.Enums;

namespace CleanMadeira.Web.ViewModels.Maintenance;

public class MaintenanceDetailsVM
{
    public Guid Id { get; set; }

    public Guid PropertyId { get; set; }

    public Guid? AssignedUserId { get; set; }

    public string PropertyName { get; set; } = string.Empty;

    public string? PropertyAddress { get; set; }

    public string? AssignedUserName { get; set; }

    public string Title { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public MaintenancePriority Priority { get; set; }

    public MaintenanceStatus Status { get; set; }

    public DateTime ScheduledDate { get; set; }

    public DateTime CreatedAt { get; set; }

    public bool HasAssignedUser =>
        AssignedUserId.HasValue &&
        !string.IsNullOrWhiteSpace(AssignedUserName);

    public bool IsOverdue =>
        ScheduledDate < DateTime.Now &&
        Status != MaintenanceStatus.Completo &&
        Status != MaintenanceStatus.Cancelado;
}