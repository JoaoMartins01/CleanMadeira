using CleanMadeira.Domain.Entities;

namespace CleanMadeira.Application.Common.Interfaces
{
    public interface ITaskPhotoRepository
    : IRepository<TaskPhoto>
    {
        Task<IEnumerable<TaskPhoto>>
            GetByTaskAsync(Guid taskId);
    }
}
