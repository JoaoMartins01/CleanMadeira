using CleanMadeira.Application.Common.Interfaces;
using CleanMadeira.Application.Services.Interface;
using CleanMadeira.Domain.Entities;
using CleanMadeira.Domain.Entities.Enums;
using Microsoft.AspNetCore.Hosting;
using WhiteLagoon.Domain.Entities;

namespace CleanMadeira.Application.Services.Implementation
{
    public class CleaningTaskService : ICleaningTaskService
    {
        private readonly ICleaningTaskRepository _taskRepository;
        private readonly IFileStorageService _fileStorageService;

        public CleaningTaskService(ICleaningTaskRepository taskRepository, IFileStorageService fileStorageService)
        {
            _taskRepository = taskRepository;
            _fileStorageService = fileStorageService;
        }

        public async Task<IEnumerable<CleaningTask>> GetAllAsync()
        {
            return await _taskRepository.GetAllAsync();
        }

        public async Task<CleaningTask?> GetByIdAsync(Guid id)
        {
            return await _taskRepository.GetByIdAsync(id);
        }

        public async Task<IEnumerable<CleaningTask>> GetTodayTasksAsync()
        {
            return await _taskRepository.GetTodayTasksAsync();
        }

        public async Task CreateAsync(CleaningTask task)
        {
            task.Id = Guid.NewGuid();
            task.Status = CleaningStatus.Pendente;

            await _taskRepository.AddAsync(task);
            await _taskRepository.SaveChangesAsync();
        }

        public async Task UpdateAsync(CleaningTask task)
        {
            await _taskRepository.UpdateAsync(task);
            await _taskRepository.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var task = await _taskRepository.GetByIdAsync(id);

            if (task == null)
                return;

            await _taskRepository.DeleteAsync(task);
            await _taskRepository.SaveChangesAsync();
        }

        public async Task StartAsync(Guid id)
        {
            var task = await _taskRepository.GetByIdAsync(id);

            if (task == null)
                return;

            task.Status = CleaningStatus.EmProgresso;
            task.StartedAt = DateTime.UtcNow;

            await _taskRepository.UpdateAsync(task);
            await _taskRepository.SaveChangesAsync();
        }

        public async Task CompleteAsync(Guid id)
        {
            var task = await _taskRepository.GetByIdAsync(id);

            if (task == null)
                return;

            task.Status = CleaningStatus.Completo;
            task.CompletedAt = DateTime.UtcNow;

            await _taskRepository.UpdateAsync(task);
            await _taskRepository.SaveChangesAsync();
        }

        public async Task<IEnumerable<CleaningTask>> GetByLimpadorUserIdAsync(Guid cleanerUserId)
        {
            return await _taskRepository.GetByLimpadorUserIdAsync(cleanerUserId);
        }

        public async Task<IEnumerable<CleaningTask>> GetByOwnerIdAsync(Guid ownerId)
        {
            return await _taskRepository.GetByOwnerIdAsync(ownerId);
        }

        public async Task<CleaningTask?> GetByIdAndOwnerAsync(Guid id, Guid ownerId)
        {
            return await _taskRepository.GetByIdAndOwnerAsync(id, ownerId);
        }

        public async Task<bool> HasOpenTasksByPropertyIdAsync(Guid propriedadeId)
        {
            return await _taskRepository
                .HasOpenTasksByPropertyIdAsync(propriedadeId);
        }

        public async Task AddCleanerUpdateAsync(Guid taskId,
            string? cleanerNotes,
              List<TaskPhoto> photos)
        {
            await _taskRepository.AddCleanerUpdateAsync(
                taskId,
                cleanerNotes,
                photos);
        }

        public async Task<bool> DeletePhotoAsync(Guid photoId, Guid taskId)
        {
            var photo = await _taskRepository
                .GetPhotoAsync(photoId, taskId);

            if (photo == null)
                return false;

            await _fileStorageService.DeleteFileAsync(photo.FileUrl);

            await _taskRepository.DeletePhotoAsync(photo);

            return true;
        }


    }
}
