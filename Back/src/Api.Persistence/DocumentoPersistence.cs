using Api.Domain;
using System.Linq;
using Api.Domain.Enums;
using System.Threading.Tasks;
using Api.Application.Contratos;
using Microsoft.EntityFrameworkCore;
using Api.Persistence.Contexto;
using Api.Application.Models;
using System;
using System.Collections.Generic;

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
            IQueryable<Documento> query = _context.Documentos.AsNoTracking();
            if (pageParams.Categoria != null) query = query.Where(d => ((int)d.Categoria) == pageParams.Categoria);

            if (!string.IsNullOrWhiteSpace(pageParams.Term))
            {
                var termoFts = pageParams.Term.Trim().Replace("'", "''");
                var idsMatch = await _context.Database
                    .SqlQuery<int>($"SELECT Id FROM DocumentoFts WHERE DocumentoFts MATCH {termoFts}")
                    .ToListAsync();
                query = query.Where(d => idsMatch.Contains(d.Id));
            }

            query = query.OrderBy(d => d.Id);

            return await PageList<Documento>.CreateAsync(query.ToList(), pageParams.PageNumber, pageParams.PageSize);
        }

        public async Task<Documento> GetDocumentoByIdAsync(int DocumentoId)
        {
            IQueryable<Documento> query = _context.Documentos;
            query = query.OrderBy(d => d.Id).Where(d => d.Id == DocumentoId);
            return await query.FirstOrDefaultAsync();
        }

        public async Task<PageList<Documento>> GetAllDocumentosByCategoriaAsync(Categoria categoria, PageParams pageParams)        {
            IQueryable<Documento> query = _context.Documentos.AsNoTracking();

            query = query.Where(d => d.Categoria == categoria);

            if (!string.IsNullOrWhiteSpace(pageParams.Term))
            {
                var termo = pageParams.Term.ToLower();
                query = query.Where(d =>
                    d.Area.ToLower().Contains(termo)
                    || d.PalavrasChave.ToLower().Contains(termo)
                    || d.Titulo.ToLower().Contains(termo)
                    || d.Autor.ToLower().Contains(termo)
                    || d.Resumo.ToLower().Contains(termo));
            }

            query = query.OrderBy(d => d.Id);

            return await PageList<Documento>.CreateAsync(query.ToList(), pageParams.PageNumber, pageParams.PageSize);
        }


        public async Task<PageList<Documento>> GetDocumentosByFiltroAsync(PageParams pageParams)
        {
            IQueryable<Documento> query = _context.Documentos.AsNoTracking();

            if (!string.IsNullOrWhiteSpace(pageParams.Ano)) query = query.Where(d => d.Ano.Contains(pageParams.Ano));
            if (!string.IsNullOrWhiteSpace(pageParams.Area)) query = query.Where(d => d.Area.Contains(pageParams.Area));

            query = query.OrderBy(d => d.Id);

            return await PageList<Documento>.CreateAsync(query.ToList(), pageParams.PageNumber, pageParams.PageSize);
        }
    }
}