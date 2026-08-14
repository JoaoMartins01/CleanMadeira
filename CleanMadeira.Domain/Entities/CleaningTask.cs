using CheckListItem.Domain.Entities;
using CleanMadeira.Domain.Entities.Enums;

namespace CleanMadeira.Domain.Entities
{
    public class CleaningTask
    {
        public Guid Id { get; set; }

        public Guid PropertyId { get; set; }

        public Guid? AssignedUserId { get; set; }

        public ApplicationUser? AssignedUser { get; set; }
        public Guid? CleaningCompanyId { get; set; }

        public Company? CleaningCompany { get; set; }

        public DateTime ScheduledDate { get; set; }

        public DateTime? StartedAt { get; set; }

        public DateTime? CompletedAt { get; set; }

        public CleaningStatus Status { get; set; }

        public ICollection<TaskPhoto> Photos { get; set; }
            = new List<TaskPhoto>();

        public ICollection<ChecklistItem> ChecklistItems { get; set; }
            = new List<ChecklistItem>();

        public TaskPriority Priority { get; set; }

        public string? Notes { get; set; }

        public string? CleanerNotes { get; set; }

        public int EstimatedMinutes { get; set; }

        public DateTime? StartTime { get; set; }

        public DateTime? EndTime { get; set; }

        public int? ActualMinutes { get; set; }

        public Property? Property { get; set; }

        public CleaningType? CleaningType { get; set; }
    }
}
