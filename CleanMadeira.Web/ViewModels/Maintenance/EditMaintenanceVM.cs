using CleanMadeira.Domain.Entities.Enums;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace CleanMadeira.Web.ViewModels.Maintenance;

public class EditMaintenanceVM
{
    [Required]
    public Guid Id { get; set; }

    [Required(ErrorMessage = "Selecione uma propriedade.")]
    [Display(Name = "Propriedade")]
    public Guid? PropriedadeId { get; set; }

    [Display(Name = "Responsável")]
    public Guid? AssignedUserId { get; set; }

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

    [Required(ErrorMessage = "Selecione um estado.")]
    [Display(Name = "Estado")]
    public MaintenanceStatus Status { get; set; }

    [Required(ErrorMessage = "A data prevista é obrigatória.")]
    [Display(Name = "Data prevista")]
    [DataType(DataType.DateTime)]
    public DateTime ScheduledDate { get; set; }

    public IEnumerable<SelectListItem> Properties { get; set; }
        = Enumerable.Empty<SelectListItem>();

    public IEnumerable<SelectListItem> AssignedUsers { get; set; }
        = Enumerable.Empty<SelectListItem>();
}
