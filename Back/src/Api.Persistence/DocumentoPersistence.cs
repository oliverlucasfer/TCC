using Api.Domain;
using System.Linq;
using Api.Domain.Enums;
using System.Threading.Tasks;
using Api.Persistence.Contratos;
using Microsoft.EntityFrameworkCore;
using Api.Persistence.Contexto;
using Api.Persistence.Models;
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
            IQueryable<Documento> query = _context.Documentos;
            if (pageParams.Categoria != null) query = query.AsNoTracking().Where(d => ((int)d.Categoria) == pageParams.Categoria);
            query = query.AsNoTracking().OrderBy(d => d.Id);

            var list = new List<Documento>();

            query.ToList().ForEach(d =>
            {
                if (BuscarFrase(pageParams.Term, d.DocumentoText, 3, 0.75))
                {
                    list.Add(d);
                }
            });

            return await PageList<Documento>.CreateAsync(list, pageParams.PageNumber, pageParams.pageSize);
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
            return await PageList<Documento>.CreateAsync(query.ToList(), pageParams.PageNumber, pageParams.pageSize);
        }


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

    }
}