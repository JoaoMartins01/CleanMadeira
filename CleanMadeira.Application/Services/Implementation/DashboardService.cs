using CleanMadeira.Application.Common.DTO;
using CleanMadeira.Application.Common.Interfaces;
using CleanMadeira.Application.Services.Interface;
using CleanMadeira.Domain.Entities.Enums;
//using CleanMadeira.Domain.Enums;

namespace CleanMadeira.Application.Services;

public class DashboardService : IDashboardService
{
    private readonly ICleaningTaskRepository _cleaningTaskRepositorio;
    private readonly IPropertyRepository _propertyRepository;
    private readonly IUtilizadorRepositorio _utilizadorRepositorio;
    private readonly IInventoryRepository _inventoryRepositorio;

    public DashboardService(
            ICleaningTaskRepository cleaningTaskRepositorio,
            IPropertyRepository propertyRepository,
            IUtilizadorRepositorio utilizadorRepositorio,
            IInventoryRepository inventoryRepositorio)
    {
        _cleaningTaskRepositorio = cleaningTaskRepositorio;
        _propertyRepository = propertyRepository;
        _utilizadorRepositorio = utilizadorRepositorio;
        _inventoryRepositorio = inventoryRepositorio;
    }

    public async Task<DashboardDto> GetDashboardAsync(Guid ownerId)
    {
        var tasks = await _cleaningTaskRepositorio.GetByOwnerIdAsync(ownerId);
        var properties = await _propertyRepository.GetByUserIdAsync(ownerId);
        var lowStockItems = await _inventoryRepositorio.GetLowStockByOwnerIdAsync(ownerId);

        var today = DateTime.Today;
        var tomorrow = today.AddDays(1);

        var todayTasks = tasks
            .Where(t => t.ScheduledDate >= today &&
                    t.ScheduledDate < tomorrow)
            .ToList();

        var upcomingTasks = tasks
                .Where(t => t.ScheduledDate > DateTime.Now)
                .OrderBy(t => t.ScheduledDate)
                .Take(5)
                .ToList();

        return new DashboardDto
        {
            TotalTasksToday = todayTasks.Count,

            PendingTasks = todayTasks.Count(t =>
                t.Status == CleaningStatus.Pendente),

            InProgressTasks = todayTasks.Count(t =>
                t.Status == CleaningStatus.EmProgresso),

            CompletedTasks = todayTasks.Count(t =>
                t.Status == CleaningStatus.Completo),

            TotalProperties = properties.Count(),

            TotalUsers = 0,

            TodayTasks = todayTasks.ToList(),

            UpcomingTasks = upcomingTasks.ToList(),

            LowStockItems = lowStockItems.ToList()
        };
    }
}