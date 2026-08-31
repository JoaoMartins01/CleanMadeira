using System.ComponentModel.DataAnnotations;

namespace CleanMadeira.Web.ViewModels.Company
{
    public class InviteManagerVM
    {
        public Guid CompanyId { get; set; }

        public string CompanyName { get; set; } = string.Empty;

        [Required(ErrorMessage = "O email é obrigatório.")]
        [EmailAddress(ErrorMessage = "Introduza um email válido.")]
        public string Email { get; set; } = string.Empty;
    }
}