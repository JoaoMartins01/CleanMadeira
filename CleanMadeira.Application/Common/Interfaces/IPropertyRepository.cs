using CleanMadeira.Domain.Entities;

namespace CleanMadeira.Application.Common.Interfaces
{
    public interface IPropertyRepository
    : IRepository<Property>
    {
        Task<IEnumerable<Property>> GetByApplicationUserAsync(Guid guid);
        Task<IEnumerable<Property>> GetByUserIdAsync(Guid guid);
        Task<Property?> GetByIdWithOwnerAsync(Guid id);
        Task<Property?> GetByIdAndOwnerIdAsync(Guid id, Guid ownerId);
        Task<Property?> GetAccessibleByIdAsync(Guid propertyId, Guid userId, Guid? companyId);
        Task<List<Property>> GetAccessiblePropertiesAsync(Guid userId, Guid? companyId);
        Task<List<Property>> GetInactiveAsync(Guid guid);
        Task<int> CountInactiveAsync(Guid id);
    }
}
