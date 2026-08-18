using System.ComponentModel.DataAnnotations;
using CleanMadeira.Domain.Enums;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CleanMadeira.Web.ViewModels.Maintenance;

public class CreateMaintenanceFromReportVM
{
    public Guid MaintenanceReportId { get; set; }

    public Guid? PropertyId { get; set; }

    public string ReportTitle { get; set; } = string.Empty;

    public string ReportDescription { get; set; } = string.Empty;

    public string? ReportLocation { get; set; }

    public string PropertyName { get; set; } = string.Empty;

    public string? PropertyAddress { get; set; }

    public string ReportedByName { get; set; } = string.Empty;

    public DateTime ReportedAt { get; set; }

    [Required(ErrorMessage = "O título é obrigatório.")]
    [StringLength(150)]
    public string Title { get; set; } = string.Empty;

    [Required(ErrorMessage = "A descrição é obrigatória.")]
    [StringLength(2000)]
    public string Description { get; set; } = string.Empty;

    [Required(ErrorMessage = "Seleciona uma data.")]
    [Display(Name = "Data prevista")]
    public DateTime ScheduledDate { get; set; }

    [Required(ErrorMessage = "Seleciona uma prioridade.")]
    public MaintenancePriority? Priority { get; set; }

    [Required(ErrorMessage = "Selecione um prestador")]
    [Display(Name = "Prestador")]
    public Guid? MaintenanceProviderId { get; set; }

    [StringLength(2000)]
    [Display(Name = "Notas do gestor")]
    public string? ManagerNotes { get; set; }

    public IEnumerable<SelectListItem> MaintenanceProviders { get; set; }
        = Enumerable.Empty<SelectListItem>();
}