using System.ComponentModel.DataAnnotations;

namespace CleanMadeira.Web.ViewModels.MaintenanceProvider
{
    public class MaintenanceProviderVM
    {
        public Guid Id { get; set; }

        [Required(ErrorMessage = "O nome é obrigatório.")]
        [Display(Name = "Nome")]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "A especialidade é obrigatória.")]
        [Display(Name = "Especialidade")]
        [StringLength(100)]
        public string Specialty { get; set; } = string.Empty;

        [Display(Name = "Empresa")]
        [StringLength(100)]
        public string? CompanyName { get; set; }

        [Required(ErrorMessage = "O telefone é obrigatório.")]
        [Display(Name = "Telefone")]
        [Phone]
        public string PhoneNumber { get; set; } = string.Empty;

        [Display(Name = "Email")]
        [EmailAddress]
        public string? Email { get; set; }

        [Display(Name = "Disponível")]
        public bool Active { get; set; } = true;

        [Display(Name = "Observações")]
        [StringLength(500)]
        public string? Notes { get; set; }
    }
}