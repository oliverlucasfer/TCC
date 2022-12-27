using Api.Application.Dtos;
using Api.Domain.Enums;

namespace Api.Aplication.Dtos
{
    public class DocumentoDto
    {
        public int Id { get; set; }
        public string Autor { get; set; }
        public string Titulo { get; set; }
        public string Area { get; set; }
        public Categoria Categoria { get; set; }
        public string PalavrasChave { get; set; }
        public string Resumo { get; set; }
        public string DocumentoURL { get; set; }
        public string DocumentoText { get; set; }
        public string Ano { get; set; }
    }
}