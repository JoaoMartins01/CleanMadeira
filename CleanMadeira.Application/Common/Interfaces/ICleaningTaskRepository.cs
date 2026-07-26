using CleanMadeira.Application.Common.DTO;
using CleanMadeira.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using WhiteLagoon.Domain.Entities;

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
        Task<CleaningTask?> GetByIdAsync(Guid id);
        Task<IEnumerable<CleaningTask>> GetByOwnerIdAsync(Guid ownerId);
        Task<CleaningTask?> GetByIdAndOwnerAsync(Guid id, Guid ownerId);
        Task<TaskPhoto?> GetPhotoAsync(Guid photoId, Guid taskId);
        Task DeletePhotoAsync(TaskPhoto photo);
    }
}
