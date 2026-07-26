namespace CleanMadeira.Web.ViewModels.CleaningTask
{
    public class AssignCleanerVM
    {
        public Guid CleaningTaskId { get; set; }

        public string PropriedadeNome { get; set; } = string.Empty;

        public DateTime ScheduledDate { get; set; }

        public string? Search { get; set; }

        public Guid? SelectedCleanerId { get; set; }

        public List<CleanerSearchItemVM> Cleaners { get; set; } = new();
    }
}