using System;
using Microsoft.AspNetCore.Mvc;
using Api.Application.Contratos;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using System.Threading;
using Api.Domain.Enums;
using Api.Application.Dtos;
using Microsoft.AspNetCore.Authorization;
using Api.Extensions;
using Api.Application.Models;
using Api.Infrastructure;

namespace Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DocumentosController : ControllerBase
    {
        private readonly IDocumentoService _documentoService;
        private readonly IBackupService _backupService;

        public DocumentosController(IDocumentoService documentoService, IBackupService backupService)
        {
            _documentoService = documentoService;
            _backupService = backupService;
        }

        [Authorize(Roles = "Administrador")]
        [HttpGet("backup")]
        public async Task<IActionResult> Backup(CancellationToken cancellationToken)
        {
            var (stream, nomeArquivo) = await _backupService.CriarBackupAsync(cancellationToken);

            var contentDisposition = new System.Net.Http.Headers.ContentDispositionHeaderValue("attachment")
            {
                FileName = nomeArquivo,
                FileNameStar = nomeArquivo
            };
            Response.Headers["content-disposition"] = contentDisposition.ToString();

            return File(stream, "application/zip", nomeArquivo);
        }

        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] PageParams pageParams)
        {
            var documentos = await _documentoService.GetAllDocumentosAsync(pageParams);
            if (documentos == null) return NoContent();

            Response.AddPagination(documentos.CurrentPage, documentos.PageSize, documentos.TotalCount, documentos.TotalPages);

            return Ok(documentos);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var documento = await _documentoService.GetDocumentoByIdAsync(id);
            if (documento == null) return NoContent();

            return Ok(documento);
        }

        [HttpGet("filtro")]
        public async Task<IActionResult> GetFiltro([FromQuery] string ano, [FromQuery] string area, [FromQuery] PageParams pageParams)
        {
            pageParams.Ano = ano ?? string.Empty;
            pageParams.Area = area ?? string.Empty;
            var documentos = await _documentoService.GetDocumentosByFiltroAsync(pageParams);
            if (documentos == null) return NoContent();

            Response.AddPagination(documentos.CurrentPage, documentos.PageSize, documentos.TotalCount, documentos.TotalPages);
            return Ok(documentos);
        }

        [HttpGet("{id}/download")]
        public async Task<IActionResult> Download(int id)
        {
            var info = await _documentoService.ObterArquivoParaDownloadAsync(id);
            if (info == null || !info.ArquivoExiste)
                return NotFound(new { message = "Arquivo não encontrado." });

            var stream = System.IO.File.OpenRead(info.CaminhoArquivo);

            var nomeSeguro = System.IO.Path.GetFileName(info.NomeOriginal);
            var contentDisposition = new System.Net.Http.Headers.ContentDispositionHeaderValue("attachment")
            {
                FileName = nomeSeguro,
                FileNameStar = nomeSeguro
            };
            Response.Headers["content-disposition"] = contentDisposition.ToString();

            return File(stream, "application/pdf", nomeSeguro);
        }

        [HttpGet("categoria")]
        public async Task<IActionResult> GetByCategoria([FromQuery] Categoria categoria, [FromQuery] PageParams pageParams)
        {
            var documentos = await _documentoService.GetAllDocumentosByCategoriaAsync(categoria, pageParams);
            if (documentos == null) return NoContent();

            Response.AddPagination(documentos.CurrentPage, documentos.PageSize, documentos.TotalCount, documentos.TotalPages);

            return Ok(documentos);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Post(DocumentoDto model)
        {
            model.DocumentoURL = "vazio";
            model.DocumentoText = model.DocumentoURL;
            var documento = await _documentoService.AddDocumento(model);
            if (documento == null) return NoContent();

            return Ok(documento);
        }

        [Authorize]
        [HttpPost("upload-documento/{documentoId}")]
        public async Task<IActionResult> UploadDocumento(int documentoId)
        {
            if (Request.Form.Files == null || Request.Form.Files.Count == 0)
                throw new ApiException("Nenhum arquivo enviado.", 400, "Arquivo obrigatório");

            var documentoRetorno = await _documentoService.UploadArquivoAsync(documentoId, Request.Form.Files[0]);
            if (documentoRetorno == null) return NoContent();

            return Ok(documentoRetorno);
        }

        [Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, DocumentoDto model)
        {
            var documento = await _documentoService.UpdateDocumento(id, model);
            if (documento == null) return NoContent();

            return Ok(documento);
        }

        [Authorize(Roles = "Administrador")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            if (await _documentoService.DeleteDocumento(id))
                return Ok(new { message = "Deletado" });

            return NoContent();
        }
    }
}