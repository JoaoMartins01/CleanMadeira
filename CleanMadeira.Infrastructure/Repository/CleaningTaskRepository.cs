using CleanMadeira.Application.Common.Interfaces;
using CleanMadeira.Domain.Entities;
using CleanMadeira.Domain.Enums;
using CleanMadeira.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

public class CleaningTaskRepository
    : Repository<CleaningTask>,
      ICleaningTaskRepository
{
    public CleaningTaskRepository(
        ApplicationDbContext context)
        : base(context)
    {
    }

    public async Task<IEnumerable<CleaningTask>>
        GetTodayTasksAsync()
    {
        var today = DateTime.Today;

        return await _context.CleaningTasks
            .Include(x => x.Property)
            .Include(x => x.AssignedUser)
            .Where(x =>
                x.ScheduledDate.Date ==
                today)
            .ToListAsync();
    }

    public async Task<IEnumerable<CleaningTask>>
        GetByUserAsync(Guid userId)
    {
        return await _context.CleaningTasks
            .Include(x => x.Property)
            .Where(x =>
                x.AssignedUserId ==
                userId)
            .ToListAsync();
    }

    public async Task<IEnumerable<CleaningTask>>
        GetByPropertyAsync(Guid propriedadeId)
    {
        return await _context.CleaningTasks
            .Where(x =>
                x.PropertyId ==
                propriedadeId)
            .ToListAsync();
    }

    public async Task<IEnumerable<CleaningTask>> GetByLimpadorUserIdAsync(Guid cleanerUserId)
    {
        return await _context.CleaningTasks
            .Include(t => t.Property)
            .Include(t => t.AssignedUser)
            .Where(t => t.AssignedUserId == cleanerUserId)
            .OrderBy(t => t.ScheduledDate)
            .ToListAsync();
    }

    

    public async Task<IEnumerable<CleaningTask>> GetByOwnerIdAsync(Guid ownerId)
    {
        return await _context.CleaningTasks
            .Include(t => t.Property)
            .Include(t => t.AssignedUser)
            .Where(t => t.Property.ApplicationUserId == ownerId &&
                   t.Property.Active)
            .OrderBy(t => t.ScheduledDate)
            .ToListAsync();
    }

    public async Task<IEnumerable<CleaningTask>> GetByCompanyIdAsync(Guid CompanyId)
    {
        return await _context.CleaningTasks
            .Include(t => t.Property)
            .Include(t => t.AssignedUser)
            .Where(t => t.CleaningCompanyId == CompanyId &&
                   t.Property.Active)
            .OrderBy(t => t.ScheduledDate)
            .ToListAsync();
    }

    public async Task<CleaningTask?> GetByIdAndOwnerIdAsync(Guid id, Guid ownerId)
    {
        return await _context.CleaningTasks
            .Include(t => t.Property)
            .Include(t => t.AssignedUser)
            .Include(t => t.Photos)
            .FirstOrDefaultAsync(x =>
                x.Id == id &&
                x.Property.ApplicationUserId == ownerId);
    }

    public async Task<CleaningTask?> GetByIdAndCleanerIdAsync(Guid id, Guid CleanerId)
    {
        return await _context.CleaningTasks
            .Include(t => t.Property)
            .Include(t => t.Property.ApplicationUser)
            .Include(t => t.AssignedUser)
            .Include(t => t.Photos)
            .FirstOrDefaultAsync(t => 
                t.Id == id &&
                t.AssignedUserId == CleanerId);
    }

    public async Task AddCleanerUpdateAsync(
    Guid taskId,
    string? cleanerNotes,
    List<TaskPhoto> photos)
    {
        var task = await _context.CleaningTasks
            .FirstOrDefaultAsync(t => t.Id == taskId);

        if (task == null)
            return;

        task.CleanerNotes = cleanerNotes;

        if (photos != null && photos.Any())
        {
            await _context.TaskPhotos.AddRangeAsync(photos);
        }

        await _context.SaveChangesAsync();
    }

    public async Task<bool> HasOpenTasksByPropertyIdAsync(Guid propriedadeId)
    {
        return await _context.CleaningTasks.AnyAsync(t =>
            t.PropertyId == propriedadeId &&
            (t.Status == CleaningStatus.Pendente ||
             t.Status == CleaningStatus.EmProgresso));
    }

    public async Task<TaskPhoto?> GetPhotoAsync(
    Guid photoId,
    Guid taskId)
    {
        return await _context.TaskPhotos
            .FirstOrDefaultAsync(p =>
                p.Id == photoId &&
                p.CleaningTaskId == taskId);
    }

    public async Task DeletePhotoAsync(TaskPhoto photo)
    {
        _context.TaskPhotos.Remove(photo);

        await _context.SaveChangesAsync();
    }
}
