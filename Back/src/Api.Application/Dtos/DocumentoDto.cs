using System.ComponentModel.DataAnnotations;
using Api.Domain.Enums;

namespace Api.Application.Dtos
{
    public class DocumentoDto
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Autor é obrigatório")]
        public string Autor { get; set; }

        [Required(ErrorMessage = "Título é obrigatório")]
        public string Titulo { get; set; }

        public string Area { get; set; }

        public string PalavrasChave { get; set; }

        public string Resumo { get; set; }

        public string DocumentoURL { get; set; }

        public string DocumentoText { get; set; }

        public string Ano { get; set; }

        [Required(ErrorMessage = "Categoria é obrigatoria")]
        public Categoria Categoria { get; set; }
    }
}