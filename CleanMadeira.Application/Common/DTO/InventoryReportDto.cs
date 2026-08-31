using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleanMadeira.Application.Common.DTO
{
    public class InventoryReportDto
    {
        public int TotalItems { get; set; }

        public int LowStockItems { get; set; }

        public int OutOfStockItems { get; set; }

        public int PropertiesWithInventory { get; set; }

        public List<InventoryReportItemDto> Items { get; set; }
            = new();
    }
}
