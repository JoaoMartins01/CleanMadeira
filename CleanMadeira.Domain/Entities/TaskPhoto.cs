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
