using CleanMadeira.Domain.Enums;
using System.ComponentModel.DataAnnotations;

public class CreateCleaningTaskVM
{
    [Required(ErrorMessage = "Selecione uma propriedade.")]
    [Display(Name = "Propriedade")]
    public Guid PropriedadeId { get; set; }

    [Display(Name = "Funcionário(a)")]
    public Guid? AssignedUserId { get; set; }

    [Required(ErrorMessage = "Indique a data da tarefa.")]
    [Display(Name = "Data Agendada")]
    public DateTime ScheduledDate { get; set; }

    [Required(ErrorMessage = "Selecione uma prioridade.")]
    [Display(Name = "Prioridade")]
    public TaskPriority Prioridade { get; set; }
    [Display(Name = "Tipo de Serviço")]
    public CleaningType TipoServico { get; set; }

    [Range(0, 1440, ErrorMessage = "O tempo estimado deve estar entre 0 e 1440 minutos.")]
    [Display(Name = "Tempo Estimado (minutos)")]
    public int EstimatedMinutes { get; set; }

    [Display(Name = "Notas")]
    public string? Notas { get; set; }

}