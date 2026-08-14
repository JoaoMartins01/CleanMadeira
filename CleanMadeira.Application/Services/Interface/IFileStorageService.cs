namespace CleanMadeira.Application.Services.Interface
{
    public interface IFileStorageService
    {
        Task DeleteFileAsync(string fileUrl);
    }
}
