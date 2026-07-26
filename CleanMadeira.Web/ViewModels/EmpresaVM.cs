using System.ComponentModel.DataAnnotations;

namespace CleanMadeira.Web.ViewModels.Empresa;

public class EmpresaVM
{
    public Guid Id { get; set; }

    [Required(ErrorMessage = "O nome da empresa é obrigatório.")]
    [Display(Name = "Nome")]
    public string Nome { get; set; } = string.Empty;

    [Display(Name = "NIF")]
    public string? Nif { get; set; }

    [EmailAddress(ErrorMessage = "Email inválido.")]
    [Display(Name = "Email")]
    public string? Email { get; set; }

    [Phone(ErrorMessage = "Telefone inválido.")]
    [Display(Name = "Telefone")]
    public string? Telefone { get; set; }

    [Display(Name = "Morada")]
    public string? Endereço { get; set; }

    [Display(Name = "Ativa")]
    public bool Active { get; set; } = true;

}
