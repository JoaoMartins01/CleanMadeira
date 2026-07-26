using CleanMadeira.Application.Common.Interfaces;
using CleanMadeira.Domain.Entities;
using CleanMadeira.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using SendGrid.Helpers.Mail;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using WhiteLagoon.Domain.Entities;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

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
