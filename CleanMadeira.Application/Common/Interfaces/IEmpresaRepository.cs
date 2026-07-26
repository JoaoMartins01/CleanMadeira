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
    public interface ICompanyRepository
    : IRepository<Company>
    {
    }
}
