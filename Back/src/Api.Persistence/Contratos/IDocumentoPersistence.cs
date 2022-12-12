using Api.Domain;
using Api.Domain.Enums;
using Api.Persistence.Models;
using System.Threading.Tasks;

namespace Api.Persistence.Contratos
{
    public interface IDocumentoPersistence
    {
        Task<PageList<Documento>> GetAllDocumentosAsync(PageParams pageParams);
        Task<Documento> GetDocumentoByIdAsync(int DocumentoId);
        Task<PageList<Documento>> GetAllDocumentosByCategoriaAsync(Categoria categoria, PageParams pageParams);
    }
}