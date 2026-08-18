using CleanMadeira.Domain.Entities;

namespace CleanMadeira.Application.Common.Interfaces
{
    public interface ICleaningTaskRepository
    : IRepository<CleaningTask>
    {
        Task<IEnumerable<CleaningTask>>
            GetTodayTasksAsync();

        Task<IEnumerable<CleaningTask>>
            GetByUserAsync(Guid userId);

        Task<IEnumerable<CleaningTask>>
            GetByPropertyAsync(Guid propertyId);
        Task<IEnumerable<CleaningTask>> GetByLimpadorUserIdAsync(Guid cleanerUserId);
        Task<bool> HasOpenTasksByPropertyIdAsync(Guid propriedadeId);
        Task AddCleanerUpdateAsync(Guid taskId, string? cleanerNotes, List<TaskPhoto> photos);
        Task<IEnumerable<CleaningTask>> GetByOwnerIdAsync(Guid ownerId);
        Task<IEnumerable<CleaningTask>> GetByCompanyIdAsync(Guid companyId);
        Task<CleaningTask?> GetByIdAndOwnerIdAsync(Guid id, Guid ownerId);
        Task<CleaningTask?> GetByIdAndCleanerIdAsync(Guid id, Guid cleanerId);
        Task<TaskPhoto?> GetPhotoAsync(Guid photoId, Guid taskId);
        Task DeletePhotoAsync(TaskPhoto photo);
    }
}
