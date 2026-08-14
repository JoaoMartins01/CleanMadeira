namespace CleanMadeira.Domain.Entities
{
    public class AuditLog
    {
        public Guid Id { get; set; }

        public Guid? UserId { get; set; }

        public string EntityName { get; set; }

        public string EntityId { get; set; }

        public string Action { get; set; }

        public string OldValue { get; set; }

        public string NewValue { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
