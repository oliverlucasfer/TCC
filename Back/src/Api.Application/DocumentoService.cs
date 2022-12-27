using System;
using Api.Domain;
using Api.Domain.Enums;
using System.Threading.Tasks;
using Api.Application.Contratos;
using Api.Persistence.Contratos;
using AutoMapper;
using Api.Aplication.Dtos;
using Api.Persistence.Models;

namespace Api.Application
{
    public class DocumentoService : IDocumentoService
    {
        private readonly IGeralPersistence _geralPersistence;
        private readonly IDocumentoPersistence _documentoPersistence;
        private readonly IMapper _mapper;
        public DocumentoService(IGeralPersistence geralPersistence, IDocumentoPersistence documentoPersistence, IMapper mapper)
        {
            _documentoPersistence = documentoPersistence;
            _geralPersistence = geralPersistence;
            _mapper = mapper;
        }
        public async Task<DocumentoDto> AddDocumento(DocumentoDto model)
        {
            try
            {
                var documento = _mapper.Map<Documento>(model);
                _geralPersistence.Add<Documento>(documento);

                if (await _geralPersistence.SaveChangesAsync())
                {
                    var documentoRetorno = await _documentoPersistence.GetDocumentoByIdAsync(documento.Id);
                    return _mapper.Map<DocumentoDto>(documentoRetorno);
                }
                return null;
            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message);
            }
        }

        public async Task<DocumentoDto> UpdateDocumento(int documentoId, DocumentoDto model)
        {
            try
            {
                var documento = await _documentoPersistence.GetDocumentoByIdAsync(documentoId);
                if (documento == null) return null;

                model.Id = documento.Id;

                _mapper.Map(model, documento);
                _geralPersistence.Update<Documento>(documento);

                if (await _geralPersistence.SaveChangesAsync())
                {
                    var documentoRetorno = await _documentoPersistence.GetDocumentoByIdAsync(documento.Id);

                    return _mapper.Map<DocumentoDto>(documentoRetorno);
                }
                return null;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<bool> DeleteDocumento(int documentoId)
        {
            try
            {
                var documento = await _documentoPersistence.GetDocumentoByIdAsync(documentoId);
                if (documento == null) throw new Exception("documento para delete não encontrado.");
                _geralPersistence.Delete<Documento>(documento);
                return await _geralPersistence.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<PageList<DocumentoDto>> GetAllDocumentosAsync(PageParams pageParams)
        {
            try
            {
                var documentos = await _documentoPersistence.GetAllDocumentosAsync(pageParams);
                if (documentos == null) return null;
                var resultado = _mapper.Map<PageList<DocumentoDto>>(documentos);

                resultado.CurrentPage = documentos.CurrentPage;
                resultado.TotalPages = documentos.TotalPages;
                resultado.PageSize = documentos.PageSize;
                resultado.TotalCount = documentos.TotalCount;

                return resultado;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<PageList<DocumentoDto>> GetAllDocumentosByCategoriaAsync(Categoria categoria, PageParams pageParams)
        {
            try
            {
                var documentos = await _documentoPersistence.GetAllDocumentosByCategoriaAsync(categoria, pageParams);
                if (documentos == null) return null;
                var resultado = _mapper.Map<PageList<DocumentoDto>>(documentos);

                return resultado;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<DocumentoDto> GetDocumentoByIdAsync(int documentoId)
        {
            try
            {
                var documento = await _documentoPersistence.GetDocumentoByIdAsync(documentoId);
                if (documento == null) return null;
                var resultado = _mapper.Map<DocumentoDto>(documento);

                return resultado;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}