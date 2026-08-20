using CleanMadeira.Domain.Entities;
using System.Security.Cryptography;

namespace CleanMadeira.Application.Services.Interface
{
    public interface IPropertyService : IService<Property>
    {
        Task<IEnumerable<Property>> GetByUserAsync(Guid applicationUserId);
        Task<Property?> GetByIdAndOwnerIdAsync(Guid id, Guid ownerId);
        Task UpdateCleaningSettingsAsync(Guid propertyId, Guid userId,bool autoIntermediateCleaning, int intermediateCleaningIntervalDays);
        Task<bool> ExistsAsync(string nome, Guid id);
        Task<List<Property>> GetInactiveAsync(Guid applicationUserId);
        Task<int> CountInactiveAsync(Guid applicationUserId);
    }
}
