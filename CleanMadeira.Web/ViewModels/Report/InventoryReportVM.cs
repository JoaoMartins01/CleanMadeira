namespace CleanMadeira.Web.ViewModels.Report
{
    public class InventoryReportVM
    {
        public int TotalItems { get; set; }

        public int LowStockItems { get; set; }

        public int OutOfStockItems { get; set; }

        public int PropertiesWithInventory { get; set; }

        public List<InventoryReportItemVM> Items { get; set; }
            = new();
    }
}
