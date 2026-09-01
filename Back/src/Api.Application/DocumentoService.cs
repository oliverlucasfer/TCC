using System;
using System.Collections.Generic;
using System.Linq;
using Api.Domain;
using Api.Domain.Enums;
using System.Threading.Tasks;
using Api.Application.Contratos;
using Api.Application.Dtos;
using Api.Application.Helpers;
using Api.Application.Models;
using Microsoft.AspNetCore.Http;

namespace Api.Application
{
    public class DocumentoService : IDocumentoService
    {
        private readonly IGeralPersistence _geralPersistence;
        private readonly IDocumentoPersistence _documentoPersistence;
        private readonly IFileService _fileService;

        public DocumentoService(IGeralPersistence geralPersistence, IDocumentoPersistence documentoPersistence, IFileService fileService)
        {
            _documentoPersistence = documentoPersistence;
            _geralPersistence = geralPersistence;
            _fileService = fileService;
        }

        public async Task<DocumentoDto> AddDocumento(DocumentoDto model)
        {
            try
            {
                var documento = model.ToEntity();
                _geralPersistence.Add<Documento>(documento);

                if (await _geralPersistence.SaveChangesAsync())
                {
                    var documentoRetorno = await _documentoPersistence.GetDocumentoByIdAsync(documento.Id);
                    return documentoRetorno.ToDto();
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

                documento.UpdateFrom(model);
                _geralPersistence.Update<Documento>(documento);

                if (await _geralPersistence.SaveChangesAsync())
                {
                    var documentoRetorno = await _documentoPersistence.GetDocumentoByIdAsync(documento.Id);

                    return documentoRetorno.ToDto();
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
                if (documento == null) return false;
                _geralPersistence.Delete<Documento>(documento);
                if (await _geralPersistence.SaveChangesAsync())
                {
                    _fileService.Delete(documento.DocumentoURL);
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<DocumentoDto> UploadArquivoAsync(int documentoId, IFormFile arquivo)
        {
            try
            {
                var documento = await _documentoPersistence.GetDocumentoByIdAsync(documentoId);
                if (documento == null) return null;

                var nomeAntigo = documento.DocumentoURL;
                var nomeNovo = await _fileService.SaveAsync(arquivo);

                try
                {
                    documento.DocumentoURL = nomeNovo;
                    documento.DocumentoText = _fileService.ExtractText(nomeNovo);
                    documento.NomeOriginal = arquivo.FileName;

                    _geralPersistence.Update<Documento>(documento);
                    if (await _geralPersistence.SaveChangesAsync())
                    {
                        _fileService.Delete(nomeAntigo);
                        return documento.ToDto();
                    }
                    return null;
                }
                catch (Exception)
                {
                    _fileService.Delete(nomeNovo);
                    throw;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<PageList<DocumentoReadDto>> GetAllDocumentosAsync(PageParams pageParams)
        {
            try
            {
                var documentos = await _documentoPersistence.GetAllDocumentosAsync(pageParams);
                if (documentos == null) return null;
                return documentos.ToReadDto();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<PageList<DocumentoReadDto>> GetAllDocumentosByCategoriaAsync(Categoria categoria, PageParams pageParams)
        {
            try
            {
                var documentos = await _documentoPersistence.GetAllDocumentosByCategoriaAsync(categoria, pageParams);
                if (documentos == null) return null;
                return documentos.ToReadDto();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<PageList<DocumentoReadDto>> GetDocumentosByFiltroAsync(PageParams pageParams)
        {
            try
            {
                var documentos = await _documentoPersistence.GetDocumentosByFiltroAsync(pageParams);
                if (documentos == null) return null;
                return documentos.ToReadDto();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<DocumentoReadDto> GetDocumentoByIdAsync(int documentoId)
        {
            try
            {
                var documento = await _documentoPersistence.GetDocumentoByIdAsync(documentoId);
                if (documento == null) return null;
                return documento.ToReadDto();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<DownloadInfo> ObterArquivoParaDownloadAsync(int documentoId)
        {
            try
            {
                var documento = await _documentoPersistence.GetDocumentoByIdAsync(documentoId);
                if (documento == null) return null;

                var caminho = _fileService.GetCaminhoCompleto(documento.DocumentoURL);
                var nomeOriginal = string.IsNullOrWhiteSpace(documento.NomeOriginal)
                    ? documento.DocumentoURL
                    : documento.NomeOriginal;

                return new DownloadInfo(caminho, nomeOriginal, _fileService.Exists(documento.DocumentoURL));
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
