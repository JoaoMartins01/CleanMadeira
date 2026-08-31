namespace CleanMadeira.Web.ViewModels.Report
{
    public class MonthlyCleaningReportVM
    {
        public int Year { get; set; }
        public int Month { get; set; }
        public int Total { get; set; }
        public int Completed { get; set; }
        public int Pending { get; set; }
        public int InProgress { get; set; }
        public int Cancelled { get; set; }

        public List<MonthlyCleaningReportItemVM> Tasks { get; set; }
            = new();
    }

}
