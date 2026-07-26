using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using CleanMadeira.Application.Common.Interfaces;
using CleanMadeira.Domain.Entities;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;
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
