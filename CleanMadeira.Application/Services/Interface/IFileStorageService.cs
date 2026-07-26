using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleanMadeira.Application.Services.Interface
{
    public interface IFileStorageService
    {
        Task DeleteFileAsync(string fileUrl);
    }
}
