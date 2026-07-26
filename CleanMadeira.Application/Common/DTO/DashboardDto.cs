

using CleanMadeira.Domain.Entities;

namespace CleanMadeira.Application.Common.DTO;

public class DashboardDto
{
    public int TotalTasksToday { get; set; }

    public int PendingTasks { get; set; }

    public int InProgressTasks { get; set; }

    public int CompletedTasks { get; set; }

    public int TotalProperties { get; set; }

    public int TotalUsers { get; set; }
    public List<CleaningTask> TodayTasks { get; set; } = new();
    public List<InventoryItem> LowStockItems { get; set; } = new();

    public List<CleaningTask> UpcomingTasks { get; set; } = new();
}
