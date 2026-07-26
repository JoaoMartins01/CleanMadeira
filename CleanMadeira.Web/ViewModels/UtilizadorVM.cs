using CleanMadeira.Domain.Entities.Enums;
using System.ComponentModel.DataAnnotations;
using WhiteLagoon.Domain.Entities;

namespace CleanMadeira.Web.ViewModels;

public class UtilizadorVM
{
    public Guid Id { get; set; }

    public Guid EmpresaId { get; set; }

    [Required(ErrorMessage = "O nome é obrigatório.")]
    [Display(Name = "Nome")]
    public string PrimeiroNome { get; set; } = string.Empty;

    [Required(ErrorMessage = "O apelido é obrigatório.")]
    [Display(Name = "Apelido")]
    public string UltimoNome { get; set; } = string.Empty;

    [Required(ErrorMessage = "O email é obrigatório.")]
    [EmailAddress(ErrorMessage = "Email inválido.")]
    [Display(Name = "Email")]
    public string Email { get; set; } = string.Empty;

    [Phone]
    [Display(Name = "Telefone")]
    public string? Telemovel { get; set; }

    [Display(Name = "Perfil")]
    public UserRole Role { get; set; }

    [Display(Name = "Ativo")]
    public bool Active { get; set; }

    public string NomeCompleto => $"{PrimeiroNome} {UltimoNome}";
}
