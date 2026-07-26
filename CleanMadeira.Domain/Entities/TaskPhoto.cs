using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using CleanMadeira.Domain.Entities.Enums;

namespace CleanMadeira.Domain.Entities
{
    public class TaskPhoto
    {
        public Guid Id { get; set; }

        public Guid CleaningTaskId { get; set; }

        public CleaningTask CleaningTask { get; set; }

        public string FileName { get; set; }

        public string FileUrl { get; set; }

        public PhotoType Type { get; set; }

        public DateTime UploadedAt { get; set; }
    }

}
