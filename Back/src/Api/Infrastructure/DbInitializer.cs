using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Api.Domain.Identity;
using Api.Domain.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Api.Infrastructure
{
    public class DbInitializer : IHostedService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly IConfiguration _config;
        private readonly ILogger<DbInitializer> _logger;

        public DbInitializer(IServiceScopeFactory scopeFactory, IConfiguration config, ILogger<DbInitializer> logger)
        {
            _scopeFactory = scopeFactory;
            _config = config;
            _logger = logger;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<Api.Persistence.Contexto.ApiContext>();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();

            await context.Database.EnsureCreatedAsync(cancellationToken);

            await EnsureColumnAsync(context, "Documentos", "NomeOriginal", "TEXT NOT NULL DEFAULT ''", cancellationToken);
            await EnsureDocumentoFtsAsync(context, cancellationToken);
            SeedAdmin(userManager);
        }

        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;

        private async Task EnsureColumnAsync(Api.Persistence.Contexto.ApiContext context, string tabela, string coluna, string definicao, CancellationToken cancellationToken)
        {
#pragma warning disable EF1002 // Identificadores internos constantes (sem entrada de usuário)
            var cols = await context.Database
                .SqlQueryRaw<string>("SELECT name FROM pragma_table_info({0})", tabela)
                .ToListAsync(cancellationToken);

            if (cols.Any(c => c == coluna))
                return;

            _logger.LogInformation("Adicionando coluna {Coluna} em {Tabela}...", coluna, tabela);
            await context.Database.ExecuteSqlRawAsync("ALTER TABLE \"" + tabela + "\" ADD COLUMN \"" + coluna + "\" " + definicao, cancellationToken);
#pragma warning restore EF1002
        }

        private async Task EnsureDocumentoFtsAsync(Api.Persistence.Contexto.ApiContext context, CancellationToken cancellationToken)
        {
            var exists = await context.Database
                .SqlQueryRaw<int>("SELECT COUNT(1) AS [Value] FROM sqlite_master WHERE type='table' AND name='DocumentoFts'")
                .ToListAsync(cancellationToken);

            if (exists.Count > 0 && exists[0] > 0)
                return;

            _logger.LogInformation("Criando índice de busca FTS5 (DocumentoFts)...");

            await context.Database.ExecuteSqlRawAsync(@"CREATE VIRTUAL TABLE DocumentoFts USING fts5(Id UNINDEXED, Titulo, Autor, Resumo, PalavrasChave, DocumentoText, content='Documentos', content_rowid='Id')", cancellationToken);
            await context.Database.ExecuteSqlRawAsync(@"CREATE TRIGGER DocumentoFts_ai AFTER INSERT ON Documentos BEGIN
                INSERT INTO DocumentoFts(Id, Titulo, Autor, Resumo, PalavrasChave, DocumentoText) VALUES (new.Id, new.Titulo, new.Autor, new.Resumo, new.PalavrasChave, new.DocumentoText);
            END", cancellationToken);
            await context.Database.ExecuteSqlRawAsync(@"CREATE TRIGGER DocumentoFts_ad AFTER DELETE ON Documentos BEGIN
                INSERT INTO DocumentoFts(DocumentoFts, Id, Titulo, Autor, Resumo, PalavrasChave, DocumentoText) VALUES ('delete', old.Id, old.Titulo, old.Autor, old.Resumo, old.PalavrasChave, old.DocumentoText);
            END", cancellationToken);
            await context.Database.ExecuteSqlRawAsync(@"CREATE TRIGGER DocumentoFts_au AFTER UPDATE ON Documentos BEGIN
                INSERT INTO DocumentoFts(DocumentoFts, Id, Titulo, Autor, Resumo, PalavrasChave, DocumentoText) VALUES ('delete', old.Id, old.Titulo, old.Autor, old.Resumo, old.PalavrasChave, old.DocumentoText);
                INSERT INTO DocumentoFts(Id, Titulo, Autor, Resumo, PalavrasChave, DocumentoText) VALUES (new.Id, new.Titulo, new.Autor, new.Resumo, new.PalavrasChave, new.DocumentoText);
            END", cancellationToken);
            await context.Database.ExecuteSqlRawAsync(@"INSERT INTO DocumentoFts(DocumentoFts, Id, Titulo, Autor, Resumo, PalavrasChave, DocumentoText) SELECT 'rebuild', Id, Titulo, Autor, Resumo, PalavrasChave, DocumentoText FROM Documentos", cancellationToken);

            _logger.LogInformation("Índice FTS5 criado.");
        }

        private void SeedAdmin(UserManager<User> userManager)
        {
            var adminUser = _config["SeedAdminUser"];
            var adminPass = _config["SeedAdminPass"];
            var adminEmail = _config["SeedAdminEmail"];

            if (string.IsNullOrEmpty(adminUser) || string.IsNullOrEmpty(adminPass))
            {
                _logger.LogInformation("SeedAdminUser/SeedAdminPass não configurados; pulando seed de admin.");
                return;
            }

            var existing = userManager.FindByNameAsync(adminUser).GetAwaiter().GetResult();
            if (existing != null)
            {
                _logger.LogInformation("Admin já existe; pulando seed.");
                return;
            }

            var user = new User
            {
                UserName = adminUser,
                Email = adminEmail ?? $"{adminUser}@example.com",
                PrimeiroNome = "Administrador",
                UltimoNome = "Sistema",
                Tipo = Tipo.Administrador
            };

            var result = userManager.CreateAsync(user, adminPass).GetAwaiter().GetResult();
            if (result.Succeeded)
                _logger.LogInformation("Admin inicial criado: {User}", adminUser);
            else
                _logger.LogWarning("Falha ao criar admin inicial: {Errors}", string.Join("; ", result.Errors.Select(e => e.Description)));
        }
    }
}