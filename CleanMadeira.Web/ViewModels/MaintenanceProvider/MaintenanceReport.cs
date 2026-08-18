using System.ComponentModel.DataAnnotations;
using CleanMadeira.Domain.Enums;
using Microsoft.AspNetCore.Http;

namespace CleanMadeira.Web.ViewModels.Maintenance;

public class MaintenanceReportVM
{
    public Guid CleaningTaskId { get; set; }

    public Guid PropertyId { get; set; }

    public string? PropertyName { get; set; }

    public string? PropertyAddress { get; set; }

    [Required(ErrorMessage = "O título é obrigatório.")]
    [StringLength(150)]
    [Display(Name = "Título")]
    public string Title { get; set; } = string.Empty;

    [StringLength(100)]
    [Display(Name = "Local do problema")]
    public string? Location { get; set; }

    [Required(ErrorMessage = "A descrição é obrigatória.")]
    [StringLength(2000)]
    [Display(Name = "Descrição")]
    public string Description { get; set; } = string.Empty;

    [Required(ErrorMessage = "Seleciona uma prioridade.")]
    [Display(Name = "Prioridade")]
    public MaintenancePriority? Priority { get; set; }

    [Display(Name = "Fotografias")]
    public List<IFormFile> Photos { get; set; } = new();
}