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
