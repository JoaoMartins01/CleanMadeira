namespace CleanMadeira.Web.ViewModels.Report
{
    public class MonthlyMaintenanceReportItemVM
    {
        public Guid Id { get; set; }

        public string Title { get; set; } = string.Empty;

        public string PropertyName { get; set; } = string.Empty;

        public string? ProviderName { get; set; }

        public DateTime ScheduledDate { get; set; }

        public string Status { get; set; } = string.Empty;

        public string Priority { get; set; } = string.Empty;
    }
}
