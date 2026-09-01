using System.ComponentModel.DataAnnotations;
using Api.Domain.Enums;

namespace Api.Application.Dtos
{
    public class UserDto
    {
        [Required(ErrorMessage = "Usuário é obrigatório")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Usuário deve ter entre 3 e 50 caracteres")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "E-mail é obrigatório")]
        [EmailAddress(ErrorMessage = "E-mail inválido")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Senha é obrigatória")]
        [StringLength(100, MinimumLength = 8, ErrorMessage = "Senha deve ter no mínimo 8 caracteres")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Nome é obrigatório")]
        public string PrimeiroNome { get; set; }

        public string UltimoNome { get; set; }

        public Tipo Tipo { get; set; } = Tipo.UsuarioComum;
    }
}