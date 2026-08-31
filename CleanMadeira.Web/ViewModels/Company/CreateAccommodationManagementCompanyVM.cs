using System.ComponentModel.DataAnnotations;

namespace CleanMadeira.Web.ViewModels.Company
{
    public class CreateAccommodationManagementCompanyVM
    {
        [Required(ErrorMessage = "O nome da empresa é obrigatório.")]
        [StringLength(150)]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "O NIF é obrigatório.")]
        [StringLength(20)]
        public string NIF { get; set; } = string.Empty;

        [Required(ErrorMessage = "O email é obrigatório.")]
        [EmailAddress(ErrorMessage = "Introduza um email válido.")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "O telefone é obrigatório.")]
        [Phone(ErrorMessage = "Introduza um número de telefone válido.")]
        [StringLength(20)]
        public string Phone { get; set; } = string.Empty;

        [Required(ErrorMessage = "A morada é obrigatória.")]
        [StringLength(250)]
        public string Address { get; set; } = string.Empty;
    }
}