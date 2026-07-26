using CleanMadeira.Domain.Entities;
using CleanMadeira.Web.ViewModels.CleaningTask;
using CleanMadeira.Web.ViewModels.Inventory;

namespace CleanMadeira.Web.ViewModels.Dashboard;

public class DashboardVM
{
    public int TotalTasksToday { get; set; }

    public int PendingTasks { get; set; }

    public int InProgressTasks { get; set; }

    public int CompletedTasks { get; set; }

    public int TotalProperties { get; set; }

    public int TotalUsers { get; set; }

    public List<CleaningTaskVM> TodayTasks { get; set; }
        = new();

    public List<CleaningTaskVM> UpcomingTasks { get; set; }
        = new();

    public List<InventoryItem> LowStockItems { get; set; }
        = new();
}