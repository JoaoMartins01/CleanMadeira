using CleanMadeira.Domain.Entities.Enums;
using System.ComponentModel.DataAnnotations;

namespace CleanMadeira.Web.ViewModels.PublicMaintenance;

public class PublicMaintenanceVM
{
    public Guid Id { get; set; }

    public Guid AccessToken { get; set; }

    public string PropertyName { get; set; } = string.Empty;

    public string? PropertyAddress { get; set; }

    public string ProviderName { get; set; } = string.Empty;

    public string Title { get; set; } = string.Empty;

    public string? Description { get; set; }

    public MaintenancePriority Priority { get; set; }

    public MaintenanceStatus Status { get; set; }

    public DateTime ScheduledDate { get; set; }

    [StringLength(2000)]
    [Display(Name = "Observações")]
    public string? ProviderNotes { get; set; }
}