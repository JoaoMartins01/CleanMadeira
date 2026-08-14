using CleanMadeira.Domain.Entities.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleanMadeira.Domain.Entities
{
    public class MaintenanceReport
    {
        public Guid Id { get; set; }

        public Guid? CleaningTaskId { get; set; }
        public CleaningTask? CleaningTask { get; set; }
        public Guid? PropertyId { get; set; }
        public Property? Property { get; set; }
        public Guid? ReportedByUserId { get; set; }
        public ApplicationUser? ReportedByUser { get; set; }
        public Guid? MaintenanceId { get; set; }
        public Maintenance? Maintenance { get; set; }

        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;

        public MaintenancePriority Priority { get; set; }
        public MaintenanceReportStatus Status { get; set; }

        public DateTime ReportedAt { get; set; }
    }
}
