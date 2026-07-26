using CleanMadeira.Application.Services.Interface;
using CleanMadeira.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;

public class CleanerNumberGenerator : ICleanerNumberGenerator
{
    private readonly ApplicationDbContext _context;

    public CleanerNumberGenerator(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<int> GenerateAsync()
    {
        const int min = 100000;
        const int max = 1000000;

        for (var attempt = 0; attempt < 20; attempt++)
        {
            var number = RandomNumberGenerator.GetInt32(min, max);

            var exists = await _context.Users
                .AnyAsync(u => u.CleanerNumber == number);

            if (!exists)
                return number;
        }

        throw new InvalidOperationException(
            "Não foi possível gerar um número único para o limpador.");
    }
}