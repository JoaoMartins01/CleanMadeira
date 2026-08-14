namespace CleanMadeira.Application.Services.Interface
{
    public interface ICleanerNumberGenerator
    {
        Task<int> GenerateAsync();
    }
}
