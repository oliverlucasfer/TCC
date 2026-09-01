using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Api.Application.Contratos;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using UglyToad.PdfPig;

namespace Api.Infrastructure
{
    public class DocumentoFileService : IFileService
    {
        private readonly string _pastaPdfs;

        public DocumentoFileService(IWebHostEnvironment hostEnvironment, IConfiguration configuration)
        {
            var dataDir = configuration["DATA_DIR"];
            if (string.IsNullOrWhiteSpace(dataDir))
                dataDir = hostEnvironment.ContentRootPath;

            _pastaPdfs = Path.Combine(dataDir, "Resources", "pdfs");
            Directory.CreateDirectory(_pastaPdfs);
        }

        public async Task<string> SaveAsync(IFormFile arquivo)
        {
            var extensao = Path.GetExtension(arquivo.FileName).ToLowerInvariant();
            if (extensao != ".pdf")
                throw new Api.Application.Models.ApiException("Somente arquivos PDF são permitidos.", 400, "Arquivo inválido");
            if (arquivo.Length > 10 * 1024 * 1024)
                throw new Api.Application.Models.ApiException("O arquivo excede o limite de 10 MB.", 400, "Arquivo inválido");

            var nomeArquivo = $"{Guid.NewGuid()}{extensao}";
            var caminho = Path.Combine(_pastaPdfs, nomeArquivo);

            using (var fileStream = new FileStream(caminho, FileMode.Create))
            {
                await arquivo.CopyToAsync(fileStream);
            }

            return nomeArquivo;
        }

        public void Delete(string arquivoNome)
        {
            if (string.IsNullOrWhiteSpace(arquivoNome)) return;

            var nomeSeguro = Path.GetFileName(arquivoNome);
            var caminho = Path.Combine(_pastaPdfs, nomeSeguro);
            if (File.Exists(caminho))
                File.Delete(caminho);
        }

        public bool Exists(string arquivoNome)
        {
            if (string.IsNullOrWhiteSpace(arquivoNome)) return false;
            var nomeSeguro = Path.GetFileName(arquivoNome);
            return File.Exists(Path.Combine(_pastaPdfs, nomeSeguro));
        }

        public string GetCaminhoCompleto(string arquivoNome)
        {
            var nomeSeguro = Path.GetFileName(arquivoNome);
            return Path.Combine(_pastaPdfs, nomeSeguro);
        }

        public string ExtractText(string arquivoNome)
        {
            var caminho = Path.Combine(_pastaPdfs, Path.GetFileName(arquivoNome));
            var texto = new StringBuilder();
            using (PdfDocument documento = PdfDocument.Open(caminho))
            {
                foreach (var pagina in documento.GetPages())
                {
                    texto.Append(pagina.Text);
                }
            }
            return texto.ToString();
        }
    }
}