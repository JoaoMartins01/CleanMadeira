using System.ComponentModel.DataAnnotations;

namespace CleanMadeira.Web.ViewModels.Inventory;

public class InventoryItemVM
{
    public Guid Id { get; set; }

    [Required(ErrorMessage = "A propriedade é obrigatória.")]
    [Display(Name = "Propriedade")]
    public Guid PropriedadeId { get; set; }
    public string? PropriedadeNome { get; set; }

    [Required(ErrorMessage = "O nome do produto é obrigatório.")]
    [Display(Name = "Produto")]
    public string Nome { get; set; } = string.Empty;

    [Required(ErrorMessage = "A unidade é obrigatória.")]
    [Display(Name = "Unidade")]
    public string Unidade { get; set; } = string.Empty;

    [Range(0, int.MaxValue)]
    [Display(Name = "Quantidade")]
    public int Quantidade { get; set; }

    [Range(0, int.MaxValue)]
    [Display(Name = "Quantidade Mínima")]
    public int QuantidadeMinima { get; set; }

    [Display(Name = "Ativo")]
    public bool Active { get; set; } = true;

    public bool IsLowStock => Quantidade <= QuantidadeMinima;
}