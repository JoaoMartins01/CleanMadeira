using CleanMadeira.Domain.Entities;

namespace CleanMadeira.Application.Services.Interface
{
    public interface IPropertyService : IService<Property>
    {
        Task<IEnumerable<Property>> GetByUserAsync(Guid applicationUserId);
        Task<Property?> GetByIdAndOwnerIdAsync(Guid id, Guid ownerId);
        Task<bool> ExistsAsync(string nome, Guid id);
        Task<List<Property>> GetInactiveAsync(Guid applicationUserId);
        Task<int> CountInactiveAsync(Guid applicationUserId);
    }
}
