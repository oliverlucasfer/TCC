using Api.Aplication.Dtos;
using Api.Domain.Enums;
using Api.Persistence.Models;
using System.Threading.Tasks;

namespace Api.Application.Contratos
{
    public interface IDocumentoService
    {
        Task<DocumentoDto> AddDocumento(DocumentoDto model);
        Task<DocumentoDto> UpdateDocumento(int documentoId, DocumentoDto model);
        Task<bool> DeleteDocumento(int documentoId);

        Task<PageList<DocumentoDto>> GetAllDocumentosAsync(PageParams pageParams);
        Task<DocumentoDto> GetDocumentoByIdAsync(int documentoId);
        Task<PageList<DocumentoDto>> GetAllDocumentosByCategoriaAsync(Categoria categoria, PageParams pageParams);
    }
}