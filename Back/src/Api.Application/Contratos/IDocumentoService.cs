using Api.Application.Dtos;
using Api.Application.Models;
using Api.Domain.Enums;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Api.Application.Contratos
{
    public interface IDocumentoService
    {
        Task<DocumentoDto> AddDocumento(DocumentoDto model);
        Task<DocumentoDto> UpdateDocumento(int documentoId, DocumentoDto model);
        Task<bool> DeleteDocumento(int documentoId);
        Task<DocumentoDto> UploadArquivoAsync(int documentoId, IFormFile arquivo);

        Task<PageList<DocumentoReadDto>> GetAllDocumentosAsync(PageParams pageParams);
        Task<DocumentoReadDto> GetDocumentoByIdAsync(int documentoId);
        Task<PageList<DocumentoReadDto>> GetAllDocumentosByCategoriaAsync(Categoria categoria, PageParams pageParams);
        Task<PageList<DocumentoReadDto>> GetDocumentosByFiltroAsync(PageParams pageParams);
        Task<DownloadInfo> ObterArquivoParaDownloadAsync(int documentoId);
    }
}
