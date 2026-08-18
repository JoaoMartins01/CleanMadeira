using CleanMadeira.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace CleanMadeira.Web.ViewModels.Account;

public class RegisterVM
{
    [Required(ErrorMessage = "O primeiro nome é obrigatório.")]
    [Display(Name = "Primeiro Nome")]
    public string PrimeiroNome { get; set; } = string.Empty;

    [Required(ErrorMessage = "O apelido é obrigatório.")]
    [Display(Name = "Apelido")]
    public string UltimoNome { get; set; } = string.Empty;

    [Required(ErrorMessage = "O email é obrigatório.")]
    [EmailAddress(ErrorMessage = "Email inválido.")]
    [Display(Name = "Email")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "O telefone é obrigatório.")]
    [Display(Name = "Telefone")]
    [RegularExpression(@"^\+?[0-9]{7,15}$",
    ErrorMessage = "Introduza um número de telefone válido.")]
    public string? Telemovel { get; set; } = string.Empty;

    [Required(ErrorMessage = "A password é obrigatória.")]
    [DataType(DataType.Password)]
    [MinLength(6, ErrorMessage = "A password deve ter pelo menos 6 caracteres.")]
    [Display(Name = "Password")]
    public string Password { get; set; } = string.Empty;

    [Required(ErrorMessage = "A confirmação da password é obrigatória.")]
    [DataType(DataType.Password)]
    [Compare("Password", ErrorMessage = "As passwords não coincidem.")]
    [Display(Name = "Confirmar Password")]
    public string ConfirmPassword { get; set; } = string.Empty;

    //[Required(ErrorMessage = "O perfil é obrigatório.")]
    public OwnerType? Type { get; set; }

    [Required(ErrorMessage = "Selecione o tipo de serviço.")]
    [Display(Name = "Serviço:")]
    public UserRole Role { get; set; }
    public string? EmpresaNome { get; set; }
}