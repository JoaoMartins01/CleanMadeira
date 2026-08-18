using CleanMadeira.Domain.Enums;
using System.ComponentModel.DataAnnotations;

public class EditCleaningTaskVM
{
    public Guid Id { get; set; }

    [Required]
    [Display(Name = "Propriedade")]
    public Guid PropriedadeId { get; set; }

    [Required]
    [Display(Name = "Data Agendada")]
    public DateTime ScheduledDate { get; set; }

    [Required]
    [Display(Name = "Prioridade")]
    public TaskPriority Prioridade { get; set; }

    [Required]
    [Display(Name = "Tipo de Serviço")]
    public CleaningType? TipoServico { get; set; }

    [Required]
    [Display(Name = "Estado")]
    public TaskStatus Status { get; set; }

    [Display(Name = "Tempo Estimado (minutos)")]
    public int EstimatedMinutes { get; set; }

    [Display(Name = "Notas")]
    public string? Notas { get; set; }

    public Guid? AssignedUserId { get; set; }
    public DateTime? StartedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
}