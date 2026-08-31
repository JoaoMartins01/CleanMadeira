namespace CleanMadeira.Web.ViewModels.Report
{
    public class InventoryReportItemVM
    {
        public Guid Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public string PropertyName { get; set; } = string.Empty;

        public decimal Quantity { get; set; }

        public decimal MinimumQuantity { get; set; }

        public string? Unit { get; set; }

        public bool IsLowStock { get; set; }

        public bool IsOutOfStock { get; set; }
    }
}
