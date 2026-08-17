using CleanMadeira.Domain.Entities;

public interface ICalendarIntegrationService
{
    Task<List<CalendarIntegration>> GetByPropertyIdAsync(
        Guid propertyId,
        Guid userId);

    Task<(bool Success, string Message)> CreateAsync(
        CalendarIntegration integration,
        Guid userId);

    Task<List<CalendarIntegration>> GetAllActiveAsync();

    Task<(bool Success, string Message)> DeleteAsync(
        Guid integrationId,
        Guid userId);

    Task<CalendarIntegration?> GetByIdAsync(Guid id);
}
