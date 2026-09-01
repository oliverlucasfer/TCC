using System.Linq;
using Api.Application.Dtos;
using Api.Domain;
using Api.Domain.Identity;
using Api.Application.Models;

namespace Api.Application.Helpers
{
    public static class MappingExtensions
    {
        public static Documento ToEntity(this DocumentoDto dto)
        {
            return new Documento
            {
                Id = dto.Id,
                Titulo = dto.Titulo,
                Autor = dto.Autor,
                Area = dto.Area,
                Ano = dto.Ano,
                Categoria = dto.Categoria,
                PalavrasChave = dto.PalavrasChave,
                Resumo = dto.Resumo,
                DocumentoURL = dto.DocumentoURL,
                DocumentoText = dto.DocumentoText
            };
        }

        public static DocumentoDto ToDto(this Documento documento)
        {
            return new DocumentoDto
            {
                Id = documento.Id,
                Titulo = documento.Titulo,
                Autor = documento.Autor,
                Area = documento.Area,
                Ano = documento.Ano,
                Categoria = documento.Categoria,
                PalavrasChave = documento.PalavrasChave,
                Resumo = documento.Resumo,
                DocumentoURL = documento.DocumentoURL,
                DocumentoText = documento.DocumentoText
            };
        }

        public static PageList<DocumentoDto> ToDto(this PageList<Documento> documentos)
        {
            return new PageList<DocumentoDto>(
                documentos.Select(ToDto).ToList(),
                documentos.TotalCount,
                documentos.CurrentPage,
                documentos.PageSize);
        }

        public static DocumentoReadDto ToReadDto(this Documento documento)
        {
            return new DocumentoReadDto
            {
                Id = documento.Id,
                Titulo = documento.Titulo,
                Autor = documento.Autor,
                Area = documento.Area,
                Ano = documento.Ano,
                Categoria = documento.Categoria,
                PalavrasChave = documento.PalavrasChave,
                Resumo = documento.Resumo,
                DocumentoURL = documento.DocumentoURL
            };
        }

        public static PageList<DocumentoReadDto> ToReadDto(this PageList<Documento> documentos)
        {
            return new PageList<DocumentoReadDto>(
                documentos.Select(ToReadDto).ToList(),
                documentos.TotalCount,
                documentos.CurrentPage,
                documentos.PageSize);
        }

        public static void UpdateFrom(this Documento documento, DocumentoDto dto)
        {
            documento.Titulo = dto.Titulo;
            documento.Autor = dto.Autor;
            documento.Area = dto.Area;
            documento.Ano = dto.Ano;
            documento.Categoria = dto.Categoria;
            documento.PalavrasChave = dto.PalavrasChave;
            documento.Resumo = dto.Resumo;
        }

        public static User ToEntity(this UserDto dto)
        {
            return new User
            {
                UserName = dto.UserName,
                Email = dto.Email,
                PrimeiroNome = dto.PrimeiroNome,
                UltimoNome = dto.UltimoNome,
                Tipo = dto.Tipo
            };
        }

        public static UserDto ToDto(this User user)
        {
            return new UserDto
            {
                UserName = user.UserName,
                Email = user.Email,
                PrimeiroNome = user.PrimeiroNome,
                UltimoNome = user.UltimoNome,
                Tipo = user.Tipo
            };
        }

        public static UserReturnDto ToReturnDto(this User user)
        {
            return new UserReturnDto
            {
                UserName = user.UserName,
                Email = user.Email,
                PrimeiroNome = user.PrimeiroNome,
                UltimoNome = user.UltimoNome,
                Tipo = user.Tipo
            };
        }

        public static UserUpdateDto ToUpdateDto(this User user)
        {
            return new UserUpdateDto
            {
                Id = user.Id,
                UserName = user.UserName,
                PrimeiroNome = user.PrimeiroNome,
                UltimoNome = user.UltimoNome,
                Email = user.Email
            };
        }

        public static void UpdateFrom(this User user, UserUpdateDto dto)
        {
            user.UserName = dto.UserName;
            user.PrimeiroNome = dto.PrimeiroNome;
            user.UltimoNome = dto.UltimoNome;
            user.Email = dto.Email;
        }
    }
}
