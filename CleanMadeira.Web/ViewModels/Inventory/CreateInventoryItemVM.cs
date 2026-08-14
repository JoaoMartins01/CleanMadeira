using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace CleanMadeira.Web.ViewModels.Inventory;

public class CreateInventoryItemVM
{
    [Required(ErrorMessage = "A propriedade é obrigatória.")]
    [Display(Name = "Propriedade")]
    public Guid? PropriedadeId { get; set; }

    [Required(ErrorMessage = "O nome do produto é obrigatório.")]
    [Display(Name = "Produto")]
    public string Nome { get; set; } = string.Empty;

    [Required(ErrorMessage = "A unidade é obrigatória.")]
    [Display(Name = "Unidade")]
    public string Unidade { get; set; } = string.Empty;

    [Required(ErrorMessage = "A quantidade é obrigatória.")]
    [Range(0, int.MaxValue)]
    [Display(Name = "Quantidade")]
    public int? Quantidade { get; set; }

    [Required(ErrorMessage = "A quantidade mínima é obrigatória.")]
    [Range(0, int.MaxValue)]
    [Display(Name = "Quantidade Mínima")]
    public int? QuantidadeMinima { get; set; }
    public List<SelectListItem> Propriedades { get; set; } = new();
}