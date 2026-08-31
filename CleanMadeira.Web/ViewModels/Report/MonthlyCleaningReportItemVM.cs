namespace CleanMadeira.Web.ViewModels.Report;

public class MonthlyCleaningReportItemVM
{
    public Guid Id { get; set; }

    public string PropertyName { get; set; } = string.Empty;

    public string? CleanerName { get; set; }

    public DateTime ScheduledDate { get; set; }

    public string Status { get; set; } = string.Empty;

    public string Priority { get; set; } = string.Empty;
}