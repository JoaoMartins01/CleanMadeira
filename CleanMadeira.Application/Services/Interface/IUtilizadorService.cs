using CleanMadeira.Domain.Entities;

namespace CleanMadeira.Application.Services.Interface
{
    public interface IUtilizadorService : IService<Utilizador>
    {
        Task<Utilizador?> GetByEmailAsync(string email);
    }
}
