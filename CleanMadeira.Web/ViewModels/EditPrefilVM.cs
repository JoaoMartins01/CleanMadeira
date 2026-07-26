using System.ComponentModel.DataAnnotations;

namespace CleanMadeira.Web.ViewModels
{
    public class EditPerfilVM
    {
        [Required]
        public string PrimeiroNome { get; set; } = string.Empty;

        [Required]
        public string UltimoNome { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Phone]
        public string? Telemovel { get; set; }

        public string? EmpresaNome { get; set; }

        public string? LimpadorCodigo { get; set; }
    }
}
