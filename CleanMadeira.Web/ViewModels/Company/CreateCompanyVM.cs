using CleanMadeira.Domain.Entities.Enums;
using System.ComponentModel.DataAnnotations;

namespace CleanMadeira.Web.ViewModels.Company
{
    public class CreateCompanyVM
    {
        [Required(ErrorMessage = "Indique o nome da empresa.")]
        [Display(Name = "Nome da empresa")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Indique o Endereço")]
        [Display(Name = "Endereço")]
        public string Endereço { get; set; } = string.Empty;

        [Required(ErrorMessage = "Indique o NIF.")]
        [Display(Name = "NIF")]
        public string Nif { get; set; } = string.Empty;

        [Required(ErrorMessage = "Indique o email.")]
        [EmailAddress(ErrorMessage = "Introduza um email válido.")]
        public string Email { get; set; } = string.Empty;

        [Display(Name = "Telefone")]
        public string? Phone { get; set; }

        [Required]
        [Display(Name = "Tipo de empresa")]
        public CompanyType Type { get; set; }
    }
}