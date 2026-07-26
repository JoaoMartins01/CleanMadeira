using System.ComponentModel.DataAnnotations;

namespace CleanMadeira.Web.ViewModels.Inventory
{
    public class InventoryCheckVM
    {
        public Guid CleaningTaskId { get; set; }

        public Guid PropriedadeId { get; set; }

        public string PropriedadeNome { get; set; } = string.Empty;

        [Display(Name = "Observações Gerais")]
        public string? Observacoes { get; set; }

        public List<InventoryCheckItemVM> Items { get; set; } = new();
    }
}
