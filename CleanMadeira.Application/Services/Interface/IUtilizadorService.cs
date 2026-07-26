using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CleanMadeira.Domain.Entities;

namespace CleanMadeira.Application.Services.Interface
{
    public interface IUtilizadorService : IService<Utilizador>
    {
        Task<Utilizador?> GetByEmailAsync(string email);
    }
}
