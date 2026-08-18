using CleanMadeira.Domain.Entities;

namespace CleanMadeira.Application.Services.Interface
{
    public interface ICleaningTaskService : IService<CleaningTask>
    {
        Task<IEnumerable<CleaningTask>> GetTodayTasksAsync();
        Task StartAsync(Guid id);
        Task CompleteAsync(Guid id);
        Task<IEnumerable<CleaningTask>> GetByLimpadorUserIdAsync(Guid cleanerUserId);
        Task<IEnumerable<CleaningTask>> GetByOwnerIdAsync(Guid ownerId);
        Task<IEnumerable<CleaningTask>> GetByCompanyIdAsync(Guid companyId);
        Task<CleaningTask?> GetByIdAndOwnerIdAsync(Guid id, Guid ownerId);
        Task<CleaningTask?> GetByIdAndCleanerIdAsync(Guid id, Guid cleanerId);
        Task<CleaningTask?> GetByIdAsync(Guid? id);
        Task AddCleanerUpdateAsync(Guid taskId, string? cleanerNotes, List<TaskPhoto> photos);
        Task<bool> HasOpenTasksByPropertyIdAsync(Guid propriedadeId);

        Task<bool> DeletePhotoAsync(Guid photoId, Guid taskId);
    }
}