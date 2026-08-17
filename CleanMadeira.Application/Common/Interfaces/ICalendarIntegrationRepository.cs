using CleanMadeira.Domain.Entities;
using CleanMadeira.Domain.Entities.Enums;

public interface ICalendarIntegrationRepository
{
    Task<List<CalendarIntegration>> GetByPropertyIdAsync(
        Guid propertyId);

    Task<CalendarIntegration?> GetByIdAsync(Guid id);

    Task<CalendarIntegration?> GetByProviderAsync(
        Guid propertyId,
        CalendarProvider provider);

    Task<List<CalendarIntegration>> GetAllActiveAsync();
    Task AddAsync(CalendarIntegration integration);

    Task DeleteAsync(CalendarIntegration integration);

    Task SaveChangesAsync();
}
