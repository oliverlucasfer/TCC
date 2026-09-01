using System.IO.Compression;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Api.Persistence.Contexto;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Api.Infrastructure
{
    public interface IBackupService
    {
        Task<(Stream stream, string nomeArquivo)> CriarBackupAsync(CancellationToken cancellationToken);
    }

    public class BackupService : IBackupService
    {
        private readonly IConfiguration _config;
        private readonly ApiContext _context;
        private readonly string _dataDir;
        private readonly string _pastaPdfs;

        public BackupService(IConfiguration config, ApiContext context)
        {
            _config = config;
            _context = context;
            _dataDir = config["DATA_DIR"] ?? Directory.GetCurrentDirectory();
            _pastaPdfs = Path.Combine(_dataDir, "Resources", "pdfs");
        }

        public async Task<(Stream stream, string nomeArquivo)> CriarBackupAsync(CancellationToken cancellationToken)
        {
            var tempDir = Path.Combine(Path.GetTempPath(), $"prodocs-backup-{Guid.NewGuid():N}");
            Directory.CreateDirectory(tempDir);

            var dbDestino = Path.Combine(tempDir, "Api.db");
            var dbFonte = _context.Database.GetDbConnection().DataSource;

            await _context.Database.ExecuteSqlRawAsync($"VACUUM INTO '{dbDestino.Replace("'", "''")}'", cancellationToken);

            var zipStream = new MemoryStream();
            using (var archive = new ZipArchive(zipStream, ZipArchiveMode.Create, leaveOpen: true))
            {
                archive.CreateEntryFromFile(dbDestino, "Api.db");

                if (Directory.Exists(_pastaPdfs))
                {
                    foreach (var pdf in Directory.GetFiles(_pastaPdfs, "*.pdf"))
                    {
                        archive.CreateEntryFromFile(pdf, Path.Combine("pdfs", Path.GetFileName(pdf)));
                    }
                }
            }

            zipStream.Position = 0;

            try { Directory.Delete(tempDir, true); } catch { }

            var nomeArquivo = $"prodocs-backup-{DateTime.Now:yyyyMMdd-HHmmss}.zip";
            return (zipStream, nomeArquivo);
        }
    }
}