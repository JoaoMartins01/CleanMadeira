using CleanMadeira.Domain.Enums;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace CleanMadeira.Web.ViewModels.Maintenance;

public class CreateMaintenanceVM
{
    [Required(ErrorMessage = "Selecione uma propriedade.")]
    [Display(Name = "Propriedade")]
    public Guid PropertyId { get; set; }

    [Required(ErrorMessage = "Adicione um responsável.")]
    [Display(Name = "Responsável")]
    public Guid AssignedUserId { get; set; }

    [Required(ErrorMessage = "O título é obrigatório.")]
    [StringLength(
        150,
        MinimumLength = 3,
        ErrorMessage = "O título deve ter entre 3 e 150 caracteres.")]
    [Display(Name = "Título")]
    public string Title { get; set; } = string.Empty;

    [Required(ErrorMessage = "A descrição é obrigatória.")]
    [StringLength(
        2000,
        ErrorMessage = "A descrição não pode ultrapassar 2000 caracteres.")]
    [Display(Name = "Descrição")]
    public string Description { get; set; } = string.Empty;

    [Required(ErrorMessage = "Selecione uma prioridade.")]
    [Display(Name = "Prioridade")]
    public MaintenancePriority Priority { get; set; }
        = MaintenancePriority.Media;

    [Required(ErrorMessage = "Selecione um estado.")]
    [Display(Name = "Estado")]
    public MaintenanceStatus Status { get; set; }
        = MaintenanceStatus.Pendente;

    [Required(ErrorMessage = "A data prevista é obrigatória.")]
    [Display(Name = "Data prevista")]
    [DataType(DataType.DateTime)]
    public DateTime ScheduledDate { get; set; }
        = DateTime.Now;

    public IEnumerable<SelectListItem> Properties { get; set; }
        = Enumerable.Empty<SelectListItem>();

    public IEnumerable<SelectListItem> AssignedUsers { get; set; }
        = Enumerable.Empty<SelectListItem>();
}
