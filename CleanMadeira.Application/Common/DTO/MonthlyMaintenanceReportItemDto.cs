using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleanMadeira.Application.Common.DTO
{
    public class MonthlyMaintenanceReportItemDto
    {
        public Guid Id { get; set; }

        public string Title { get; set; } = string.Empty;

        public string PropertyName { get; set; } = string.Empty;

        public string? ProviderName { get; set; }

        public DateTime ScheduledDate { get; set; }

        public string Status { get; set; } = string.Empty;

        public string Priority { get; set; } = string.Empty;
    }
}
