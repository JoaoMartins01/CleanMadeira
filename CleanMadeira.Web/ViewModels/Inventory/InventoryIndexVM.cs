using Microsoft.AspNetCore.Mvc.Rendering;

namespace CleanMadeira.Web.ViewModels.Inventory
{
    public class InventoryIndexVM
    {
        public Guid? PropriedadeSelecionadaId { get; set; }

        public List<SelectListItem> Propriedades { get; set; } = new();

        public List<InventoryItemVM> Produtos { get; set; } = new();
    }
}
