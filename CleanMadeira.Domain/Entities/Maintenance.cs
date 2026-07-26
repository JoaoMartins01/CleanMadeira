using CleanMadeira.Domain.Entities.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleanMadeira.Domain.Entities
{
    public class Maintenance
    {
        public Guid Id { get; set; }
        public Guid AccessToken { get; set; }
        public Guid PropertyId { get; set; }
        public Property Property { get; set; } = null!;

        public Guid? AssignedUserId { get; set; }
        public ApplicationUser? AssignedUser { get; set; }
        public Guid? MaintenanceProviderId { get; set; }
        public MaintenanceProvider? MaintenanceProvider { get; set; }

        public string Title { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public MaintenancePriority Priority { get; set; }

        public MaintenanceStatus Status { get; set; }

        public DateTime ScheduledDate { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
