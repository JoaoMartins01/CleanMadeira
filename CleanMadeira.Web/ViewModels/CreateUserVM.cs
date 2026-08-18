//using CleanMadeira.Domain.Enums;
using CleanMadeira.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace CleanMadeira.Web.ViewModels;

public class CreateUserVM
{
    [Required(ErrorMessage = "O nome é obrigatório.")]
    [StringLength(100)]
    public string PrimeiroNome { get; set; } = string.Empty;

    [Required(ErrorMessage = "O apelido é obrigatório.")]
    [StringLength(100)]
    public string UltimoNome { get; set; } = string.Empty;

    [Required(ErrorMessage = "O email é obrigatório.")]
    [EmailAddress(ErrorMessage = "Email inválido.")]
    public string Email { get; set; } = string.Empty;

    [Phone(ErrorMessage = "Número de telefone inválido.")]
    public string? Telemovel { get; set; }

    [Required(ErrorMessage = "A password é obrigatória.")]
    [DataType(DataType.Password)]
    [MinLength(6, ErrorMessage = "A password deve ter pelo menos 6 caracteres.")]
    public string Password { get; set; } = string.Empty;

    [Required(ErrorMessage = "A confirmação da password é obrigatória.")]
    [DataType(DataType.Password)]
    [Compare(nameof(Password),
        ErrorMessage = "As passwords não coincidem.")]
    public string ConfirmPassword { get; set; } = string.Empty;

    [Required(ErrorMessage = "O perfil é obrigatório.")]
    [Display(Name = "Serviço:")]
    public UserRole Role { get; set; }

    public bool Active { get; set; } = true;
}

