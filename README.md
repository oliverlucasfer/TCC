# ProDocs — Biblioteca de Documentos Acadêmicos

Aplicação web para catalogação, busca e download de documentos acadêmicos (TCCs, artigos, monografias, dissertações, teses, projetos e livros), com busca full-text no conteúdo dos PDFs (SQLite FTS5), upload de arquivos, autenticação JWT e permissões por tipo de usuário.

## Stack

| Camada | Tecnologias |
|---|---|
| Backend | .NET 10 (Web API), ASP.NET Core Identity, JWT (HMAC-SHA512), EF Core 10 + SQLite |
| Frontend | Angular 20, ngx-bootstrap 20 (Modal/Pagination), RxJS |
| Estilo | Bootstrap 5 via tema [Bootswatch Cosmo](https://bootswatch.com/cosmo/) |
| PDF | PdfPig (extração de texto) |

## Arquitetura

N-camadas com inversão de dependência: a **Application define os contratos** (services e persistência) e as camadas de baixo implementam.

```
Back/src/
├── Api/               # Host/presentation: controllers finos, Startup, Swagger, arquivos estáticos (/Resources) e infra de I/O (Infrastructure/DocumentoFileService: PDF, upload)
├── Api.Application/   # Casos de uso: services, DTOs, contratos (IDocumentoService, I*Persistence, IFileService), TokenService, PageParams/PageList, mapeamentos manuais
├── Api.Domain/        # Entidades: Documento, User/Role (Identity), enums Categoria/Tipo
└── Api.Persistence/   # EF Core: ApiContext, implementações dos repositórios (Geral/Documento/User), migrations

Front/src/
├── app/components/    # home, user (login/registro/perfil), documento (listagem/detalhe/criar-editar)
├── app/shared/        # header, sidenav (navega por route params)
├── app/services/      # account, documento
├── app/interceptors/  # anexo o JWT em todas as requisições
└── app/guard/         # AuthGuard para rotas protegidas
```

Dependências entre projetos: `Api → Api.Application + Api.Persistence`, `Api.Application → Api.Domain`, `Api.Persistence → Api.Application + Api.Domain`.

**Exceção consciente**: `Api.Domain.Identity.User` herda de `IdentityUser<int>` (ASP.NET Identity) — tradeoff intencional para reaproveitar UserManager/SignInManager/JWT, padrão de mercado em apps Identity.

## Pré-requisitos

- [.NET SDK 10](https://dotnet.microsoft.com/download) (pinado em `Back/src/global.json`)
- Node.js 22 LTS (o Angular 20 não suporta Node 18)

## Como rodar

**1. API** (http://localhost:5000 — Swagger em `/swagger` em Development):

```bash
cd Back/src/Api
dotnet run
```

**2. Frontend** (http://localhost:4200):

```bash
cd Front
npm install
ng serve
```

## Configuração obrigatória — chave JWT

Sem a chave a API **não sobe** (falha explícita na inicialização). HS512 exige chave com **pelo menos 64 bytes** (ex: `openssl rand -base64 48`).

**Desenvolvimento** (user-secrets, fora do git):

```bash
cd Back/src/Api
dotnet user-secrets set "Jwt:TokenKey" "<chave-secreta-forte-64-bytes>"
```

**Produção**: defina a variável de ambiente `Jwt__TokenKey`.

> `JwtOptions` é lido da seção `Jwt` do `appsettings.json` (Issuer/Audience/ExpirationHours); a `TokenKey` fica fora do versionamento.

## Usuários e permissões

| Tipo | Permissões |
|---|---|
| `UsuarioComum` | Buscar/listar/baixar documentos |
| `UsuarioAvancado` | + Criar e editar documentos |
| `Administrador` | + Excluir documentos e gerenciar |

- Leitura (`GET`) é pública; escrita (`POST`/`PUT`/upload) exige login; exclusão exige role `Administrador` (validada por claim no token JWT — `[Authorize(Roles = "Administrador")]`).
- Cadastro em `/user/registration` **sempre cria `UsuarioComum`** — o backend ignora o `tipo` enviado (segurança).

## Endpoints principais

**Account** (`api/Account`)

| Método | Rota | Auth | Descrição |
|---|---|---|---|
| POST | `/Register` | — | Cria conta (sempre `UsuarioComum`) |
| POST | `/Login` | — | Retorna `{userName, primeiroNome, tipo, token}` |
| GET | `/GetUser` | ✅ | Dados do usuário autenticado |
| POST | `/UpdateUser` | ✅ | Atualiza dados/senha do usuário logado |

**Documentos** (`api/Documentos`)

| Método | Rota | Auth | Descrição |
|---|---|---|---|
| GET | `/` | — | Lista paginada + busca full-text (FTS5) no texto dos PDFs (`?pageNumber&pageSize&term&categoria`) |
| GET | `/{id}` | — | Detalhe do documento |
| GET | `/categoria` | — | Lista paginada por categoria |
| GET | `/filtro` | — | Filtra por `?ano` e/ou `?area` (paginado) |
| GET | `/{id}/download` | — | Baixa o PDF original (público, `Content-Disposition` com nome original) |
| GET | `/backup` | ✅ Admin | Gera ZIP com banco + PDFs (VACUUM INTO do SQLite) |
| POST | `/` | ✅ | Cria documento |
| POST | `/upload-documento/{id}` | ✅ | Upload do PDF (somente `.pdf`, máx. 10 MB) + indexação do texto |
| PUT | `/{id}` | ✅ | Atualiza documento |
| DELETE | `/{id}` | ✅ Admin | Exclui registro + arquivo físico |

**Health** — `GET /api/health` (usado como healthcheck no Render).

> A leitura **não** expõe o texto integral dos PDFs (`DocumentoText` fica apenas no servidor para a busca FTS5). O download de arquivos é feito pelo endpoint `/{id}/download` (público); a pasta `/Resources` **não** é servida como estática.

## Testes

```bash
# Backend (xUnit)
cd Back/src
dotnet test Api.Tests

# Frontend (Karma, ChromeHeadless)
cd Front
ng test --watch=false --browsers=ChromeHeadless
```

## Banco de dados

SQLite (`Back/src/Api/Api.db`, fora do versionamento). No primeiro boot, o `DbInitializer` (hosted service) cria o schema (`EnsureCreated`), o índice de busca FTS5 (`DocumentoFts` + triggers) e, se configurado, o admin inicial.

## Deploy (Render.com — plano gratuito)

O projeto inclui `Dockerfile` (3 estágios) e `render.yaml` (Blueprint). A API serve a própria SPA em produção (`/` → `wwwroot`, fallback para `index.html`), então **um único serviço** basta.

**Passos:**

1. Envie o repositório para o GitHub.
2. No [Render.com](https://render.com), crie um **Blueprint** apontando para o `render.yaml` (ou crie um Web Service Docker manualmente com o `Dockerfile`).
3. Defina as **variáveis de ambiente** no painel (as marcadas com `sync: false` no `render.yaml` devem ser preenchidas manualmente):

| Variável | Descrição |
|---|---|
| `Jwt__TokenKey` | Chave JWT (≥ 64 bytes). **Obrigatória.** |
| `SeedAdminUser` | Usuário do admin inicial (auto-criado no boot) |
| `SeedAdminPass` | Senha do admin inicial |
| `SeedAdminEmail` | E-mail do admin inicial (opcional) |
| `ConnectionStrings__Default` | `Data Source=/data/Api.db` (já definido no `render.yaml`) |
| `DATA_DIR` | `/data` (volume) — onde ficam o SQLite e os PDFs |

4. O `render.yaml` já monta um **volume persistente** de 1 GB em `/data` (SQLite + PDFs sobrevivem a redeploys).

Se `SeedAdminUser`/`SeedAdminPass` não forem definidos, o seed é pulado.

## CI

`.github/workflows/ci.yml` — GitHub Actions roda `dotnet build`/`test` (backend) e `ng build`/`test` (frontend) em pushes para `main` e PRs.

## Limitações conhecidas

- A busca FTS5 funciona em SQLite; para volumes muito grandes considere um buscador dedicado (ex: Elasticsearch).
- SPA e API são servidos na mesma origem em produção (recomendado). Para hosts separados, ajuste `apiURL` em `environment.prod.ts`.
