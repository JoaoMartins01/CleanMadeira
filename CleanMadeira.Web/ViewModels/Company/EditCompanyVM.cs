using System.ComponentModel.DataAnnotations;

namespace CleanMadeira.Web.ViewModels.Company
{
    public class EditCompanyVM
    {
        public Guid Id { get; set; }

        [Required]
        [Display(Name = "Nome da empresa")]
        public string Name { get; set; } = string.Empty;

        [Required]
        [Display(Name = "NIF")]
        public string Nif { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Display(Name = "Telefone")]
        public string? Phone { get; set; }
    }
}
