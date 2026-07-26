using CleanMadeira.Application.Common.DTO;
using WhiteLagoon.Web.ViewModels;

namespace CleanMadeira.Application.Services.Interface;
public interface IDashboardService
{
    Task<DashboardDto> GetDashboardAsync(Guid ownerId);
}