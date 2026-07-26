using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleanMadeira.Domain.Entities
{
    public class Reservation
    {
        public Guid Id { get; set; }

        public Guid PropertyId { get; set; }
        public Property Property { get; set; } = null!;

        public Guid CalendarIntegrationId { get; set; }
        public CalendarIntegration CalendarIntegration { get; set; } = null!;

        public string ExternalUid { get; set; } = string.Empty;

        public string? Summary { get; set; }

        public DateTime CheckIn { get; set; }

        public DateTime CheckOut { get; set; }

        public bool IsCancelled { get; set; }

        public DateTime LastSyncedAt { get; set; }

        public Guid? CleaningTaskId { get; set; }

        public CleaningTask? CleaningTask { get; set; }
    }
}
