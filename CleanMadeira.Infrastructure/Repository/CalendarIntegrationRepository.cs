using CleanMadeira.Application.Common.Interfaces;
using CleanMadeira.Domain.Entities;
using CleanMadeira.Domain.Entities.Enums;
using CleanMadeira.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

public class CalendarIntegrationRepository
    : ICalendarIntegrationRepository
{
    private readonly ApplicationDbContext _context;

    public CalendarIntegrationRepository(
        ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<CalendarIntegration>>
        GetByPropertyIdAsync(Guid propertyId)
    {
        return await _context.CalendarIntegrations
            .AsNoTracking()
            .Where(x => x.PropertyId == propertyId)
            .OrderBy(x => x.Provider)
            .ToListAsync();
    }

    public async Task<CalendarIntegration?>
        GetByIdAsync(Guid id)
    {
        return await _context.CalendarIntegrations
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<CalendarIntegration?>
        GetByProviderAsync(
            Guid propertyId,
            CalendarProvider provider)
    {
        return await _context.CalendarIntegrations
            .FirstOrDefaultAsync(x =>
                x.PropertyId == propertyId &&
                x.Provider == provider);
    }

    public async Task AddAsync(
        CalendarIntegration integration)
    {
        await _context.CalendarIntegrations
            .AddAsync(integration);
    }

    public Task DeleteAsync(
        CalendarIntegration integration)
    {
        _context.CalendarIntegrations.Remove(integration);

        return Task.CompletedTask;
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}