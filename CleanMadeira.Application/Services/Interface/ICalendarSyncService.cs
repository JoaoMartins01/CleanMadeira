using CleanMadeira.Application.Models;

namespace CleanMadeira.Application.Services.Interface
{
    public interface ICalendarSyncService
    {
        Task<CalendarSyncResult> SyncAsync(
            Guid calendarIntegrationId,
            CancellationToken cancellationToken = default);
    }
}
