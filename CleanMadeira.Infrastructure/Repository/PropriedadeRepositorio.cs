using CleanMadeira.Application.Common.Interfaces;
using CleanMadeira.Domain.Entities;
using CleanMadeira.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using SendGrid.Helpers.Mail;
using Property = CleanMadeira.Domain.Entities.Property;

public class PropertyRepository
    : Repository<Property>,
      IPropertyRepository
{
    public PropertyRepository(ApplicationDbContext context)
        : base(context)
    {
    }

    public async Task<IEnumerable<Property>>
        GetByApplicationUserAsync(Guid ApplicationUserId)
    {
        return await _context.Properties
            .Where(x =>
                x.ApplicationUserId == ApplicationUserId &&
                x.Active)
            .ToListAsync();
    }

    public async Task<IEnumerable<Property>> GetByUserIdAsync(Guid userId)
    {
        return await _context.Properties
            .Where(p => p.ApplicationUserId == userId &&
                   p.Active)
            .ToListAsync();
    }

    public async Task<Property?> GetByIdWithOwnerAsync(Guid id)
    {
        return await _context.Properties
            .Include(p => p.ApplicationUser) // ou User/ApplicationUser, conforme a tua entidade
            .FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task<Property?> GetByIdAndOwnerAsync(Guid id, Guid ownerId)
    {
        return await _context.Properties
            .FirstOrDefaultAsync(x =>
                x.Id == id &&
                x.ApplicationUserId == ownerId);
    }

    public async Task<List<Property>> GetInactiveAsync(Guid userId)
    {
        return await _context.Properties
            .Where(p => p.ApplicationUserId == userId &&
                   !p.Active)
            .OrderBy(p => p.Name)
            .ToListAsync();
    }

    public async Task<int> CountInactiveAsync(Guid userId)
    {
        return await _context.Properties
            .CountAsync(p => p.ApplicationUserId == userId &&
                        !p.Active);
    }
}
