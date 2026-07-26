using CleanMadeira.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WhiteLagoon.Domain.Entities;

namespace CleanMadeira.Application.Services.Interface
{
    public interface ICleaningTaskService : IService<CleaningTask>
    {
        Task<IEnumerable<CleaningTask>> GetTodayTasksAsync();
        Task StartAsync(Guid id);
        Task CompleteAsync(Guid id);
        Task<IEnumerable<CleaningTask>> GetByLimpadorUserIdAsync(Guid cleanerUserId);
        Task<IEnumerable<CleaningTask>> GetByOwnerIdAsync(Guid ownerId);
        Task<CleaningTask?> GetByIdAndOwnerAsync(Guid id, Guid ownerId);
        Task AddCleanerUpdateAsync(Guid taskId, string? cleanerNotes, List<TaskPhoto> photos);
        Task<bool> HasOpenTasksByPropertyIdAsync(Guid propriedadeId);

        Task<bool> DeletePhotoAsync(Guid photoId, Guid taskId);
    }
}