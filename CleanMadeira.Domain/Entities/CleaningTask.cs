using CheckListItem.Domain.Entities;
using CleanMadeira.Domain.Entities.Enums;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WhiteLagoon.Domain.Entities;

namespace CleanMadeira.Domain.Entities
{
    public class CleaningTask
    {
        public Guid Id { get; set; }

        public Guid PropertyId { get; set; }

        public Guid? AssignedUserId { get; set; }

        public ApplicationUser? AssignedUser { get; set; }

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


    }
}
