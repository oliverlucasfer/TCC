using System;
using Microsoft.AspNetCore.Mvc;
using Api.Application.Contratos;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using Api.Domain.Enums;
using Api.Aplication.Dtos;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using System.Linq;
using Api.Extensions;
using Api.Persistence.Models;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;
using System.Text;

namespace Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DocumentosController : ControllerBase
    {
        private readonly IDocumentoService _documentoService;
        private readonly IWebHostEnvironment _hostEnvironment;
        private readonly IAccountService _accountService;

        public DocumentosController(IDocumentoService documentoService, IWebHostEnvironment hostEnvironment, IAccountService accountService)
        {
            _accountService = accountService;
            _hostEnvironment = hostEnvironment;
            _documentoService = documentoService;

        }

        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] PageParams pageParams)
        {
            try
            {
                var documentos = await _documentoService.GetAllDocumentosAsync(pageParams);
                if (documentos == null) return NoContent();

                Response.AddPagination(documentos.CurrentPage, documentos.PageSize, documentos.TotalCount, documentos.TotalPages);

                return Ok(documentos);
            }
            catch (Exception ex)
            {
                return this.StatusCode(
                    StatusCodes.Status500InternalServerError,
                    $"Erro ao tentar recuparar documentos. Erro: {ex.Message}");
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var documento = await _documentoService.GetDocumentoByIdAsync(id);
                if (documento == null) return NoContent();

                return Ok(documento);
            }
            catch (Exception ex)
            {
                return this.StatusCode(
                    StatusCodes.Status500InternalServerError,
                    $"Erro ao tentar recuparar documento. Erro: {ex.Message}");
            }
        }

        [HttpGet("categoria")]
        public async Task<IActionResult> GetByCategoria([FromQuery] Categoria categoria, PageParams pageParams)
        {
            try
            {
                var documentos = await _documentoService.GetAllDocumentosByCategoriaAsync(categoria, pageParams);
                if (documentos == null) return NoContent();

                Response.AddPagination(documentos.CurrentPage, documentos.PageSize, documentos.TotalCount, documentos.TotalPages);

                return Ok(documentos);
            }
            catch (Exception ex)
            {
                return this.StatusCode(
                    StatusCodes.Status500InternalServerError,
                    $"Erro ao tentar recuparar documento. Erro: {ex.Message}");
            }
        }

        [HttpPost]
        public async Task<IActionResult> Post(DocumentoDto model)
        {
            try
            {
                model.DocumentoURL = model.DocumentoURL.Remove(0, 12);
                var documento = await _documentoService.AddDocumento(model);
                if (documento == null) return NoContent();

                return Ok(documento);
            }
            catch (Exception ex)
            {
                return this.StatusCode(
                    StatusCodes.Status500InternalServerError,
                     $"Erro ao tentar adiconar documento. Erro: {ex.Message}");
            }
        }

        [HttpPost("upload-documento/{documentoId}")]
        public async Task<IActionResult> UploadDocumento(int documentoId)
        {
            try
            {
                var documento = await _documentoService.GetDocumentoByIdAsync(documentoId);
                if (documento == null) return NoContent();

                var file = Request.Form.Files[0];
                if (file.Length > 0)
                {
                    DeleteDocumento(documento.DocumentoURL);
                    documento.DocumentoURL = await SaveDocumento(file);
                }

                var documentoRetorno = await _documentoService.UpdateDocumento(documentoId, documento);

                return Ok(documentoRetorno);
            }
            catch (Exception ex)
            {

                return this.StatusCode(StatusCodes.Status500InternalServerError,
                    $"Erro ao tentar adicionar documento. Erro: {ex.Message}");
            }
        }

        [NonAction]
        public async Task<string> SaveDocumento(IFormFile documentoFile)
        {
            string documentoName = new String(
                System.IO.Path.GetFileNameWithoutExtension(documentoFile.FileName)
                .ToArray())
                .Replace(' ', '-');

            documentoName = $"{documentoName}{System.IO.Path.GetExtension(documentoFile.FileName)}";

            var documentoPath =
                System.IO.Path.Combine(@"Resources/pdfs", documentoName);

            using (var fileStream = new FileStream(documentoPath, FileMode.Create))
            {
                await documentoFile.CopyToAsync(fileStream);
            }

            return documentoName;

        }

        [NonAction]
        public void DeleteDocumento(string documentoName)
        {
            var documentoPath =
                System.IO.Path.Combine(@"Resources/pdfs", documentoName);
            if (System.IO.File.Exists(documentoPath))
                System.IO.File.Delete(documentoPath);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, DocumentoDto model)
        {
            try
            {
                var documento = await _documentoService.UpdateDocumento(id, model);
                if (documento == null) return NoContent();

                return Ok(documento);
            }
            catch (Exception ex)
            {
                return this.StatusCode(
                    StatusCodes.Status500InternalServerError,
                    $"Erro ao tentar atualizar documento. Erro: {ex.Message}");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var documento = await _documentoService.GetDocumentoByIdAsync(id);
                if (documento == null) return NoContent();
                if (await _documentoService.DeleteDocumento(id))
                {
                    DeleteDocumento(documento.DocumentoURL);
                    return Ok(new { message = "Deletado" });
                }
                else
                {
                    throw new Exception("Ocorreu um problem não específico ao tentar deletar Evento.");
                }
            }
            catch (Exception ex)
            {
                return this.StatusCode(
                    StatusCodes.Status500InternalServerError,
                    $"Erro ao tentar excluir documento. Erro: {ex.Message}");
            }
        }
    }
}