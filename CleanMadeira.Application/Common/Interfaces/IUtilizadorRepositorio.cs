using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using CleanMadeira.Domain.Entities;

namespace CleanMadeira.Application.Common.Interfaces
{
    public interface IUtilizadorRepositorio
    : IRepository<Utilizador>
    {
        Task<Utilizador?> GetByEmailAsync(string email);

        Task<IEnumerable<Utilizador>>
            GetByEmpresaAsync(Guid empresaId);
    }
}
