using CleanMadeira.Application.Services.Interface;
using CleanMadeira.Domain.Entities;
using CleanMadeira.Web.ViewModels.CleaningTask;
using CleanMadeira.Web.ViewModels.Dashboard;
using CleanMadeira.Web.ViewModels.Inventory;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace CleanMadeira.Web.Controllers;

[Authorize]
public class DashboardController : Controller
{
    private readonly IDashboardService _dashboardService;
    private readonly UserManager<ApplicationUser> _userManager;

    public DashboardController(
        IDashboardService dashboardService,
        UserManager<ApplicationUser> userManager)
    {
        _dashboardService = dashboardService;
        _userManager = userManager;
    }

    public async Task<IActionResult> Index()
    {
        var user = await _userManager.GetUserAsync(User);

        if (user == null)
            return RedirectToAction("Login", "Account");

        var dto = await _dashboardService.GetDashboardAsync(user.Id);

        var vm = new DashboardVM
        {
            TotalTasksToday = dto.TotalTasksToday,
            PendingTasks = dto.PendingTasks,
            InProgressTasks = dto.InProgressTasks,
            CompletedTasks = dto.CompletedTasks,
            TotalProperties = dto.TotalProperties,
            TotalUsers = dto.TotalUsers,

            TodayTasks = dto.TodayTasks.Select(t => new CleaningTaskVM
            {
                Id = t.Id,
                PropriedadeId = t.PropertyId,
                PropriedadeNome = t.Property?.Name ?? "",
                AssignedUserId = t.AssignedUserId,
                AssignedUserName = t.AssignedUser != null
                    ? $"{t.AssignedUser.PrimeiroNome} {t.AssignedUser.UltimoNome}"
                    : "Não atribuída",
                ScheduledDate = t.ScheduledDate,
                Status = t.Status,
                Prioridade = t.Priority,
                EstimatedMinutes = t.EstimatedMinutes,
                Notas = t.Notes
            }).ToList(),

            UpcomingTasks = dto.UpcomingTasks.Select(t => new CleaningTaskVM
            {
                Id = t.Id,
                PropriedadeNome = t.Property?.Name ?? "",
                AssignedUserName = t.AssignedUser != null
                    ? $"{t.AssignedUser.PrimeiroNome} {t.AssignedUser.UltimoNome}"
                    : "Não atribuída",
                ScheduledDate = t.ScheduledDate,
                Status = t.Status,
                Prioridade = t.Priority,
                EstimatedMinutes = t.EstimatedMinutes,
                Notas = t.Notes
            }).ToList(),


            LowStockItems = dto.LowStockItems.Select(i => new InventoryItem
            {
                Id = i.Id,
                PropertyId = i.PropertyId,
                Property = i.Property,
                Name = i.Name,
                Quantity = i.Quantity,
                MinimumQuantity = i.MinimumQuantity,
                Unity = i.Unity,
                Active = i.Active
            }).ToList()
        };

        return View(vm);
    }
}