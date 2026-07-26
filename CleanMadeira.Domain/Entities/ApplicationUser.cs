using CleanMadeira.Domain.Entities.Enums;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace CleanMadeira.Domain.Entities;

public class ApplicationUser : IdentityUser<Guid>
{
    public string PrimeiroNome { get; set; } = string.Empty;
    public string UltimoNome { get; set; } = string.Empty;
    public UserRole Role { get; set; }
    //   public Guid? OwnerId { get; set; }
    // public ApplicationUser? Owner { get; set; }

    public int? CleanerNumber { get; set; }

    [NotMapped]
    public string CleanerCode => CleanerNumber.HasValue
        ? $"LMP-{CleanerNumber.Value:D6}"
        : string.Empty;

    public bool Active { get; set; } = true;

    public DateTime CreatedAt { get; set; }

    // Navegação
    public OwnerType Type { get; set; }

    public String? CompanyName { get; set; }

    public ICollection<CleaningTask> CleaningTasks { get; set; }
        = new List<CleaningTask>();
}