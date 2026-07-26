using System.ComponentModel.DataAnnotations;

namespace CleanMadeira.Web.ViewModels.Account
{
    public class ForgotPasswordVM
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;
    }
}
