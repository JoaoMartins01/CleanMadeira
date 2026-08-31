using CleanMadeira.Application.Common.Interfaces;
using CleanMadeira.Application.Services.Interface;
using CleanMadeira.Domain.Entities;

public class PropertyService : IPropertyService
{
    private readonly IPropertyRepository _propertyRepository;

    public PropertyService(IPropertyRepository propertyRepository)
    {
        _propertyRepository = propertyRepository;
    }

    public async Task<IEnumerable<Property>> GetAllAsync()
    {
        return await _propertyRepository.GetAllAsync();
    }

    public async Task<Property?> GetByIdAsync(Guid? id)
    {
        return await _propertyRepository.GetByIdAsync(id);
    }

    public async Task<Property?> GetByIdAndOwnerIdAsync(Guid id, Guid ownerId)
    {
        return await _propertyRepository.GetByIdAndOwnerIdAsync(id, ownerId);
    }

    public async Task<Property?> GetAccessibleByIdAsync(
    Guid propertyId,
    Guid userId,
    Guid? companyId)
    {
        return await _propertyRepository
            .GetAccessibleByIdAsync(
                propertyId,
                userId,
                companyId);
    }

    public async Task<List<Property>> GetAccessiblePropertiesAsync(
    Guid userId,
    Guid? companyId)
    {
        return await _propertyRepository
            .GetAccessiblePropertiesAsync(
                userId,
                companyId);
    }

    public async Task CreateAsync(Property propriedade)
    {
        propriedade.Id = Guid.NewGuid();
        propriedade.Active = true;

        await _propertyRepository.AddAsync(propriedade);
        await _propertyRepository.SaveChangesAsync();
    }

    public async Task UpdateAsync(Property propriedade)
    {
        await _propertyRepository.UpdateAsync(propriedade);
        await _propertyRepository.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        var propriedade = await _propertyRepository.GetByIdAsync(id);

        if (propriedade == null)
            return;

        await _propertyRepository.DeleteAsync(propriedade);
        await _propertyRepository.SaveChangesAsync();
    }


    public async Task<bool> ExistsAsync(string nome, Guid ownerId)
    {
        var propriedades = await _propertyRepository.GetAllAsync();

        return propriedades.Any(p =>
            p.ApplicationUserId == ownerId &&
            p.Name.Trim().ToLower() == nome.Trim().ToLower());
    }

    public async Task UpdateCleaningSettingsAsync(
    Guid propertyId,
    Guid userId,
    bool autoIntermediateCleaning,
    int intermediateCleaningIntervalDays)
    {
        var property = await _propertyRepository
            .GetByIdAndOwnerIdAsync(propertyId, userId);

        if (property == null)
        {
            throw new KeyNotFoundException(
                "Propriedade não encontrada.");
        }

        property.AutoIntermediateCleaning =
            autoIntermediateCleaning;

        property.IntermediateCleaningIntervalDays =
            intermediateCleaningIntervalDays;

        await _propertyRepository.UpdateAsync(property);
    }

    public Task<IEnumerable<Property>> GetByEmpresaAsync(Guid applicationUserId)
    {
        return _propertyRepository.GetByApplicationUserAsync(applicationUserId);
    }

    public Task<IEnumerable<Property>> GetByUserAsync(Guid applicationUserId)
    {
        return _propertyRepository.GetByApplicationUserAsync(applicationUserId);
    }

    public async Task<List<Property>> GetInactiveAsync(Guid applicationUserId)
    {
        return await _propertyRepository.GetInactiveAsync(applicationUserId);
    }

    public async Task<int> CountInactiveAsync(Guid applicationUserId)
    {
        return await _propertyRepository.CountInactiveAsync(applicationUserId);
    }

    public Task AddAsync(Property entity)
    {
        return _propertyRepository.AddAsync(entity);
    }

    public Task DeleteAsync(Property entity)
    {
        return _propertyRepository.DeleteAsync(entity);
    }

    public Task SaveChangesAsync()
    {
        return _propertyRepository.SaveChangesAsync();
    }
}