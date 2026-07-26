using System.ComponentModel.DataAnnotations;

namespace CleanMadeira.Web.ViewModels.Account
{
    public class ChangePasswordVM
    {
        [Required(ErrorMessage = "A palavra-passe atual é obrigatória.")]
        [DataType(DataType.Password)]
        [Display(Name = "Palavra-passe Atual")]
        public string CurrentPassword { get; set; } = string.Empty;

        [Required(ErrorMessage = "A nova palavra-passe é obrigatória.")]
        [StringLength(100, MinimumLength = 6,
            ErrorMessage = "A palavra-passe deve ter entre 6 e 100 caracteres.")]
        [DataType(DataType.Password)]
        [Display(Name = "Nova Palavra-passe")]
        public string NewPassword { get; set; } = string.Empty;

        [Required(ErrorMessage = "Confirme a nova palavra-passe.")]
        [DataType(DataType.Password)]
        [Compare(nameof(NewPassword),
            ErrorMessage = "As palavras-passe não coincidem.")]
        [Display(Name = "Confirmar Nova Palavra-passe")]
        public string ConfirmPassword { get; set; } = string.Empty;
    }
}