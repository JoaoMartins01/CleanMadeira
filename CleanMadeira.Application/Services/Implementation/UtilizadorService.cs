using CleanMadeira.Application.Common.Interfaces;
using CleanMadeira.Application.Services.Interface;
using CleanMadeira.Domain.Entities;

public class UtilizadorService : IUtilizadorService
{
    private readonly IUtilizadorRepositorio _utilizadorRepository;

    public UtilizadorService(IUtilizadorRepositorio utilizadorRepository)
    {
        _utilizadorRepository = utilizadorRepository;
    }

    public async Task<IEnumerable<Utilizador>> GetAllAsync()
    {
        return await _utilizadorRepository.GetAllAsync();
    }

    public async Task<Utilizador?> GetByIdAsync(Guid id)
    {
        return await _utilizadorRepository.GetByIdAsync(id);
    }

    public async Task<Utilizador?> GetByEmailAsync(string email)
    {
        return await _utilizadorRepository.GetByEmailAsync(email);
    }

    public async Task CreateAsync(Utilizador utilizador)
    {
        utilizador.Id = Guid.NewGuid();
        utilizador.Active = true;
        utilizador.CreatedAt = DateTime.UtcNow;

        await _utilizadorRepository.AddAsync(utilizador);
        await _utilizadorRepository.SaveChangesAsync();
    }

    public async Task UpdateAsync(Utilizador utilizador)
    {
        await _utilizadorRepository.UpdateAsync(utilizador);
        await _utilizadorRepository.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        var utilizador = await _utilizadorRepository.GetByIdAsync(id);

        if (utilizador == null)
            return;

        await _utilizadorRepository.DeleteAsync(utilizador);
        await _utilizadorRepository.SaveChangesAsync();
    }
}