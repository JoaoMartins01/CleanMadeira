using CleanMadeira.Application.Services.Interface;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

public class CalendarSyncBackgroundService : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<CalendarSyncBackgroundService> _logger;

    public CalendarSyncBackgroundService(
        IServiceScopeFactory scopeFactory,
        ILogger<CalendarSyncBackgroundService> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(
        CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = _scopeFactory.CreateScope();

                var integrationService =
                    scope.ServiceProvider
                        .GetRequiredService<ICalendarIntegrationService>();

                var syncService =
                    scope.ServiceProvider
                        .GetRequiredService<ICalendarSyncService>();

                var integrations =
                    await integrationService.GetAllActiveAsync();

                foreach (var integration in integrations)
                {
                    try
                    {
                        await syncService.SyncAsync(
                            integration.Id,
                            stoppingToken);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(
                            ex,
                            "Erro ao sincronizar calendário {IntegrationId}",
                            integration.Id);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Erro no serviço automático de calendários");
            }

            await Task.Delay(
                TimeSpan.FromMinutes(15),
                stoppingToken);
        }
    }
}
