using System.ComponentModel.DataAnnotations;

namespace CleanMadeira.Web.ViewModels.Propriedade;

public class PropriedadeVM
{
    public Guid Id { get; set; }

    public Guid ApplicationUserId { get; set; }

    [Required(ErrorMessage = "O nome da propriedade é obrigatório.")]
    [StringLength(100, ErrorMessage = "O nome não pode ter mais de 100 caracteres.")]
    public string Nome { get; set; } = string.Empty;

    [Required(ErrorMessage = "A morada é obrigatória.")]
    [StringLength(200)]
    [Display(Name = "Endereço")]
    public string Endereco { get; set; } = string.Empty;

    [Required(ErrorMessage = "A Freguesia é obrigatória.")]
    public string Freguesia { get; set; } = string.Empty;

    [Required(ErrorMessage = "O Código Postal é obrigatório.")]
    [RegularExpression(@"^\d{4}-\d{3}$",
    ErrorMessage = "Formato: 9000-001")]
    public string CodigoPostal { get; set; } = string.Empty;

    [Required(ErrorMessage = "Selecione a localização no mapa.")]
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }

    [Display(Name = "Quartos")]
    public int Quartos { get; set; }

    [Display(Name = "Casas de Banho")]
    public int CasasBanho { get; set; }

    [Display(Name = "Numero de Hospedes")]
    public int? NumeroHospedes { get; set; }

    [Display(Name = "Descrição")]
    public string? Descricao { get; set; }

    [Display(Name = "Ativo")]
    public bool Active { get; set; } = true;
}