namespace CleanMadeira.Web.ViewModels
{
    public class InventoryCheckItemVM
    {
        public Guid InventoryItemId { get; set; }

        public string Nome { get; set; } = string.Empty;

        public int? QuantidadeAtual { get; set; }

        public int? QuantidadeMinima { get; set; }

        public string Unidade { get; set; } = string.Empty;

        public bool EmBaixoStock => QuantidadeAtual <= QuantidadeMinima;

        public bool Repor { get; set; }

        public string? Observacoes { get; set; }
    }
}
