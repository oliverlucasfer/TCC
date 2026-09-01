using Api.Application.Models;
using Api.Domain;
using Api.Domain.Enums;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Api.Application.Contratos
{
    public interface IDocumentoPersistence
    {
        Task<PageList<Documento>> GetAllDocumentosAsync(PageParams pageParams);
        Task<Documento> GetDocumentoByIdAsync(int DocumentoId);
        Task<PageList<Documento>> GetAllDocumentosByCategoriaAsync(Categoria categoria, PageParams pageParams);
        Task<PageList<Documento>> GetDocumentosByFiltroAsync(PageParams pageParams);
    }
}
