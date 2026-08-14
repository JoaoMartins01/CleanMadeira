using CleanMadeira.Domain.Entities;

namespace CheckListItem.Domain.Entities
{
    public class ChecklistItem
    {
        public Guid Id { get; set; }

        public Guid CleaningTaskId { get; set; }

        public CleaningTask CleaningTask { get; set; }

        public string Description { get; set; }

        public bool Complete { get; set; }

        public DateTime? CompletedAt { get; set; }
    }
}
