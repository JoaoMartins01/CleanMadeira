using Microsoft.EntityFrameworkCore;
using CleanMadeira.Application.Common.Interfaces;
using CleanMadeira.Domain.Entities;
using SendGrid.Helpers.Mail;
using CleanMadeira.Infrastructure.Data;

public class UtilizadorRepository
    : Repository<Utilizador>,
      IUtilizadorRepositorio
{
    public UtilizadorRepository(ApplicationDbContext context)
        : base(context)
    {
    }

    public async Task<Utilizador?>
        GetByEmailAsync(string email)
    {
        return await _context.Utilizadores
            .FirstOrDefaultAsync(x =>
                x.Email == email);
    }

    public async Task<IEnumerable<Utilizador>>
        GetByEmpresaAsync(Guid companyId)
    {
        return await _context.Utilizadores
            .Where(x =>
                x.CompanyId == companyId)
            .ToListAsync();
    }
}