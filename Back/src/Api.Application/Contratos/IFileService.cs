using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Api.Application.Contratos
{
    public interface IFileService
    {
        Task<string> SaveAsync(IFormFile arquivo);
        void Delete(string arquivoNome);
        string ExtractText(string arquivoNome);
        bool Exists(string arquivoNome);

        string GetCaminhoCompleto(string arquivoNome);
    }
}
