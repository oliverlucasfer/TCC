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
using Api.Persistence.Contexto;

namespace Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DocumentosController : ControllerBase
    {
        private readonly IDocumentoService _documentoService;
        private readonly IWebHostEnvironment _hostEnvironment;
        private readonly IAccountService _accountService;
        private readonly ApiContext _context;

        public DocumentosController(ApiContext context, IDocumentoService documentoService, IWebHostEnvironment hostEnvironment, IAccountService accountService)
        {
            _accountService = accountService;
            _hostEnvironment = hostEnvironment;
            _documentoService = documentoService;
            _context = context;

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

        [HttpGet("pdf")]
        public async Task<IActionResult> GetUltimo()
        {
            try
            {
                var documento = _context.Documentos.OrderByDescending(x => x.Id).FirstOrDefault();
                return Ok(documento);
            }
            catch (Exception ex)
            {
                return this.StatusCode(
                    StatusCodes.Status500InternalServerError,
                    $"Erro ao tentar recuparar documento. Erro: {ex.Message}");
            }
        }

        [HttpGet("filtro")]
        public async Task<IActionResult> GetFiltro([FromQuery] string ano, string area)
        {
            try
            {
                var documentos = _context.Documentos.Where(d => d.Area.Contains(area) || d.Ano.Contains(ano)).OrderBy(x => x.Id).ToList();
                return Ok(documentos);
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
                model.DocumentoURL = "vazio";
                model.DocumentoText = model.DocumentoURL;
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
                    documento.DocumentoText = ExtrairTexto(System.IO.Path.Combine(@"Resources/pdfs", documento.DocumentoURL));
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

        [NonAction]
        public string ExtrairTexto(string caminho)
        {
            using (PdfReader leitor = new PdfReader(caminho))
            {
                StringBuilder texto = new StringBuilder();
                for (int i = 1; i <= leitor.NumberOfPages; i++)
                {
                    texto.Append(PdfTextExtractor.GetTextFromPage(leitor, i));
                }
                return texto.ToString();
            }
        }

        [NonAction]
        public Boolean BuscarFrase(string frase, string pdf, int toleracia, double precisao)
        {
            char[] delimiterChars = { ' ', ',', '.', ':', '\t' };
            string[] source = pdf.Split(delimiterChars, StringSplitOptions.RemoveEmptyEntries);
            string text = frase.ToLower();
            string[] words = text.Split(delimiterChars, StringSplitOptions.RemoveEmptyEntries);
            bool[] wordChecked = new bool[words.Length];
            int fraseindex = 0;

            bool flag = false;
            int tol = toleracia;

            foreach (var wordpdf in source)
            {
                fraseindex = 0;
                flag = false;
                foreach (var word in words)
                {
                    if (wordpdf.Contains(word, StringComparison.InvariantCultureIgnoreCase))
                    {
                        wordChecked[fraseindex] = true;
                        flag = true;
                        tol = toleracia;
                    }
                    fraseindex++;
                }

                if (!flag)
                {
                    tol--;
                }
                if (tol == 0)
                {
                    for (int i = 0; i < words.Length; i++)
                    {
                        wordChecked[i] = false;
                    }
                }

                bool allcheck = true;
                int checkcount = 0;
                foreach (var check in wordChecked)
                {
                    allcheck = allcheck && check;
                    if (check)
                    {
                        checkcount++;
                    }
                }
                if (allcheck || (checkcount >= Math.Ceiling(precisao * words.Length)))
                {
                    return true;
                }
            }

            return false;
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