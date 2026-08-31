using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleanMadeira.Application.Common.DTO
{
    public class MonthlyMaintenanceReportDto
    {
        public int Year { get; set; }
        public int Month { get; set; }

        public int Total { get; set; }
        public int Pending { get; set; }
        public int Accepted { get; set; }
        public int InProgress { get; set; }
        public int Completed { get; set; }
        public int Cancelled { get; set; }
        public int Rejected { get; set; }

        public List<MonthlyMaintenanceReportItemDto> Maintenances { get; set; }
            = new();
    }
}
