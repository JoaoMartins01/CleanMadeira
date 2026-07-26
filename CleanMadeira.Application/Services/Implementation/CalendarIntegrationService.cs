using CleanMadeira.Application.Common.Interfaces;
using CleanMadeira.Domain.Entities;

public class CalendarIntegrationService
    : ICalendarIntegrationService
{
    private readonly ICalendarIntegrationRepository _repository;
    private readonly IPropertyRepository _propertyRepository;

    public CalendarIntegrationService(
        ICalendarIntegrationRepository repository,
        IPropertyRepository propertyRepository)
    {
        _repository = repository;
        _propertyRepository = propertyRepository;
    }

    public async Task<List<CalendarIntegration>>
        GetByPropertyIdAsync(
            Guid propertyId,
            Guid userId)
    {
        var property =
            await _propertyRepository.GetByIdAsync(propertyId);

        if (property is null)
            return new List<CalendarIntegration>();

        if (property.ApplicationUserId != userId)
            return new List<CalendarIntegration>();

        return await _repository
            .GetByPropertyIdAsync(propertyId);
    }

    public async Task<(bool Success, string Message)> CreateAsync(
        CalendarIntegration integration,
        Guid userId)
    {
        var propriedade =
            await _propertyRepository.GetByIdAsync(
                integration.PropertyId);

        if (propriedade is null)
        {
            return (
                false,
                "A propriedade não foi encontrada."
            );
        }

        if (propriedade.ApplicationUserId != userId)
        {
            return (
                false,
                "Não tem permissão para adicionar integrações nesta propriedade."
            );
        }

        if (string.IsNullOrWhiteSpace(integration.CalendarUrl))
        {
            return (
                false,
                "Introduza o URL do calendário."
            );
        }

        if (!Uri.TryCreate(
                integration.CalendarUrl,
                UriKind.Absolute,
                out var uri) ||
            (uri.Scheme != Uri.UriSchemeHttp &&
             uri.Scheme != Uri.UriSchemeHttps))
        {
            return (
                false,
                "O URL do calendário não é válido."
            );
        }

        var existing =
            await _repository.GetByProviderAsync(
                integration.PropertyId,
                integration.Provider);

        if (existing is not null)
        {
            return (
                false,
                "Já existe uma integração para esta plataforma."
            );
        }

        integration.Id = Guid.NewGuid();
        integration.CalendarUrl =
            integration.CalendarUrl.Trim();

        integration.IsEnabled = true;
        integration.LastSync = null;

        await _repository.AddAsync(integration);
        await _repository.SaveChangesAsync();

        return (
            true,
            "Integração adicionada com sucesso."
        );
    }

    public async Task<(bool Success, string Message)> DeleteAsync(
        Guid integrationId,
        Guid userId)
    {
        var integration =
            await _repository.GetByIdAsync(integrationId);

        if (integration is null)
        {
            return (
                false,
                "A integração não foi encontrada."
            );
        }

        var propriedade =
            await _propertyRepository.GetByIdAsync(
                integration.PropertyId);

        if (propriedade is null ||
            propriedade.ApplicationUserId != userId)
        {
            return (
                false,
                "Não tem permissão para remover esta integração."
            );
        }

        await _repository.DeleteAsync(integration);
        await _repository.SaveChangesAsync();

        return (
            true,
            "Integração removida com sucesso."
        );
    }

    public async Task<CalendarIntegration?> GetByIdAsync(Guid id)
    {
        return await _repository.GetByIdAsync(id);
    }
}