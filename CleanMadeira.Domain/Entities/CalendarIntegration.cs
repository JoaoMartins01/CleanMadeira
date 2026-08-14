using CleanMadeira.Domain.Entities.Enums;

namespace CleanMadeira.Domain.Entities
{
    public class CalendarIntegration
    {
        public Guid Id { get; set; }

        public Guid PropertyId { get; set; }

        public Property Property { get; set; } = null!;

        public CalendarProvider Provider { get; set; }

        public string CalendarUrl { get; set; } = string.Empty;

        public bool IsEnabled { get; set; }

        public DateTime? LastSync { get; set; }
    }
}
