using CleanMadeira.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
