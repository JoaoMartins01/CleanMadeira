using System.ComponentModel.DataAnnotations;
using CleanMadeira.Domain.Entities.Enums;

namespace CleanMadeira.Web.ViewModels.CleaningTask;

public class CleaningTaskVM
{
    public Guid Id { get; set; }

    [Display(Name = "Propriedade")]
    public Guid PropriedadeId { get; set; }

    public string PropriedadeNome { get; set; } = string.Empty;
    public string GestorNome { get; set; } = string.Empty;
    public string? GestorTelefone { get; set; }

    [Display(Name = "Endereco")]
    public string Morada { get; set; }
    public string Freguesia { get; set; }

    [Display(Name = "Funcionária")]
    public Guid? AssignedUserId { get; set; }

    public string? AssignedUserName { get; set; }

    public string? AssignedUserPhone { get; set; }
    public string? AssignedUserCode { get; set; }

    [Display(Name = "Data Agendada")]
    public DateTime ScheduledDate { get; set; }

    [Display(Name = "Iniciado Em")]
    public DateTime? StartedAt { get; set; }

    [Display(Name = "Concluído Em")]
    public DateTime? CompletedAt { get; set; }

    [Display(Name = "Estado")]
    public CleaningStatus Status { get; set; }

    [Display(Name = "Prioridade")]
    public TaskPriority Prioridade { get; set; }

    [Display(Name = "Duração Estimada (min)")]
    public int EstimatedMinutes { get; set; }
    public DateTime? StartTime { get; set; }

    [Display(Name = "Observações")]
    public string? Notas { get; set; }

    public string? CleanerNotes { get; set; }

    public List<CleaningPhotoVM> Photos { get; set; } = new();

    public double? Latitude { get; set; }

    public double? Longitude { get; set; }

    public int TotalPhotos { get; set; }

    public int TotalChecklistItems { get; set; }

    public int CompletedChecklistItems { get; set; }
}