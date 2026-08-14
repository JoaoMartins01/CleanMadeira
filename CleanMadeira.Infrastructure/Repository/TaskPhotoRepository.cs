using CleanMadeira.Application.Common.Interfaces;
using CleanMadeira.Domain.Entities;
using CleanMadeira.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CleanMadeira.Infrastructure.Repository
{
    public class TaskPhotoRepository
        : Repository<TaskPhoto>,
          ITaskPhotoRepository
    {
        public TaskPhotoRepository(
            ApplicationDbContext context)
            : base(context)
        {
        }

        public async Task<IEnumerable<TaskPhoto>>
            GetByTaskAsync(Guid taskId)
        {
            return await _context.TaskPhotos
                .Where(x =>
                    x.CleaningTaskId == taskId)
                .ToListAsync();
        }
    }
}
