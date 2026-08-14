using CleanMadeira.Application.Common.Interfaces;
using CleanMadeira.Domain.Entities;
using CleanMadeira.Infrastructure.Data;

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
    }
}
