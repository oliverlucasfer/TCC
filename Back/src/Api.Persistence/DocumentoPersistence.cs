using Api.Domain;
using System.Linq;
using Api.Domain.Enums;
using System.Threading.Tasks;
using Api.Persistence.Contratos;
using Microsoft.EntityFrameworkCore;
using Api.Persistence.Contexto;
using Api.Persistence.Models;

namespace Api.Persistence
{
    public class DocumentoPersistence : IDocumentoPersistence
    {
        private readonly ApiContext _context;
        public DocumentoPersistence(ApiContext context)
        {
            _context = context;
            _context.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;

        }

        public async Task<PageList<Documento>> GetAllDocumentosAsync(PageParams pageParams)
        {
            IQueryable<Documento> query = _context.Documentos;
            if (pageParams.Categoria != null) query = query.AsNoTracking().Where(d => ((int)d.Categoria) == pageParams.Categoria);
            query = query.AsNoTracking().Where(
                d => d.Area.ToLower().Contains(pageParams.Term.ToLower())
                || d.PalavrasChave.ToLower().Contains(pageParams.Term.ToLower())
                || d.Titulo.ToLower().Contains(pageParams.Term.ToLower())
                || d.Autor.ToLower().Contains(pageParams.Term.ToLower())
                || d.Resumo.ToLower().Contains(pageParams.Term.ToLower()))
                .OrderBy(d => d.Id);
            return await PageList<Documento>.CreateAsync(query, pageParams.PageNumber, pageParams.pageSize);
        }

        public async Task<Documento> GetDocumentoByIdAsync(int DocumentoId)
        {
            IQueryable<Documento> query = _context.Documentos;
            query = query.OrderBy(d => d.Id).Where(d => d.Id == DocumentoId);
            return await query.FirstOrDefaultAsync();
        }

        public async Task<PageList<Documento>> GetAllDocumentosByCategoriaAsync(Categoria categoria, PageParams pageParams)
        {
            IQueryable<Documento> query = _context.Documentos;
            query = query.AsNoTracking().OrderBy(d => d.Id).Where(
                d => d.Area.ToLower().Contains(pageParams.Term.ToLower())
                || d.PalavrasChave.ToLower().Contains(pageParams.Term.ToLower())
                || d.Titulo.ToLower().Contains(pageParams.Term.ToLower())
                || d.Autor.ToLower().Contains(pageParams.Term.ToLower())
                || d.Resumo.ToLower().Contains(pageParams.Term.ToLower())
                && d.Categoria.ToString() == "0")
                .OrderBy(d => d.Id);
            return await PageList<Documento>.CreateAsync(query, pageParams.PageNumber, pageParams.pageSize);
        }

    }
}