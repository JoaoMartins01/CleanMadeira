using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleanMadeira.Application.Common.DTO
{
    public class InventoryReportItemDto
    {
        public Guid Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public string PropertyName { get; set; } = string.Empty;

        public decimal Quantity { get; set; }

        public decimal MinimumQuantity { get; set; }

        public string? Unit { get; set; }

        public bool IsLowStock =>
            Quantity > 0 &&
            Quantity <= MinimumQuantity;

        public bool IsOutOfStock =>
            Quantity <= 0;
    }
}
