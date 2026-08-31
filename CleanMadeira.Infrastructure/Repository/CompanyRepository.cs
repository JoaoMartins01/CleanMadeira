using CleanMadeira.Application.Common.Interfaces;
using CleanMadeira.Domain.Entities;
using CleanMadeira.Domain.Enums;
using CleanMadeira.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CleanMadeira.Infrastructure.Repository
{
    public class CompanyRepository
    : Repository<Company>,
      ICompanyRepository
    {
        public CompanyRepository(ApplicationDbContext context)
            : base(context)
        {
        }
        public async Task<List<Company>> GetCleaningCompaniesAsync()
        {
            return await _context.Companies
                .AsNoTracking()
                .Where(x => x.Type == CompanyType.Limpeza)
                .OrderBy(x => x.Name)
                .ToListAsync();
        }
        public async Task<Company?> GetByIdWithMembersAsync(Guid id)
        {
            return await _context.Companies
                .AsNoTracking()
                .Include(x => x.Users)
                .Include(x => x.Properties)
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task AddInvitationAsync(
    CompanyInvitation invitation)
        {
            await _context.CompanyInvitations
                .AddAsync(invitation);

            await _context.SaveChangesAsync();
        }

        public async Task<CompanyInvitation?>
            GetInvitationByTokenAsync(Guid token)
        {
            return await _context.CompanyInvitations
                .Include(x => x.Company)
                .FirstOrDefaultAsync(x =>
                    x.Token == token);
        }

        public async Task<bool> HasPendingInvitationAsync(
            Guid companyId,
            string email)
        {
            var now = DateTime.UtcNow;

            return await _context.CompanyInvitations
                .AnyAsync(x =>
                    x.CompanyId == companyId &&
                    x.Email == email &&
                    !x.Accepted &&
                    x.ExpiresAt > now);
        }

        public async Task UpdateInvitationAsync(
            CompanyInvitation invitation)
        {
            _context.CompanyInvitations.Update(invitation);

            await _context.SaveChangesAsync();
        }
    }
}
