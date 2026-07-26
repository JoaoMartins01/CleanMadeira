using System.ComponentModel.DataAnnotations;

namespace CleanMadeira.Web.ViewModels.MaintenanceProvider
{
    public class MaintenanceProviderCreateVM
    {
        [Required(ErrorMessage = "O nome é obrigatório.")]
        [StringLength(150)]
        [Display(Name = "Nome")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "A categoria é obrigatória.")]
        [StringLength(100)]
        [Display(Name = "Categoria")]
        public string Category { get; set; } = string.Empty;

        [Required(ErrorMessage = "O telefone é obrigatório.")]
        [Phone]
        [StringLength(30)]
        [Display(Name = "Telefone")]
        public string Phone { get; set; } = string.Empty;

        [EmailAddress(ErrorMessage = "Email inválido.")]
        [StringLength(150)]
        [Display(Name = "Email")]
        public string? Email { get; set; }

        public bool Active = true;
    }
}