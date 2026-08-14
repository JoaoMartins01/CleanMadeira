using CleanMadeira.Domain.Entities;

namespace CleanMadeira.Application.Common.Interfaces
{
    public interface IPropertyRepository
    : IRepository<Property>
    {
        Task<IEnumerable<Property>> GetByApplicationUserAsync(Guid guid);
        Task<IEnumerable<Property>> GetByUserIdAsync(Guid guid);
        Task<Property?> GetByIdWithOwnerAsync(Guid id);
        Task<Property?> GetByIdAndOwnerAsync(Guid id, Guid ownerId);
        Task<List<Property>> GetInactiveAsync(Guid guid);
        Task<int> CountInactiveAsync(Guid id);
    }
}
