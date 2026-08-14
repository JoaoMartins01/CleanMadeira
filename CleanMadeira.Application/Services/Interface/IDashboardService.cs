using CleanMadeira.Application.Common.DTO;

namespace CleanMadeira.Application.Services.Interface;

public interface IDashboardService
{
    Task<DashboardDto> GetDashboardAsync(Guid ownerId);
}