using System.ComponentModel.DataAnnotations;
using Api.Application.Dtos.Validators;

namespace Api.Application.Dtos
{
    public class UserUpdateDto
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Usuário é obrigatório")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Nome é obrigatório")]
        public string PrimeiroNome { get; set; }

        public string UltimoNome { get; set; }

        [Required(ErrorMessage = "E-mail é obrigatório")]
        [EmailAddress(ErrorMessage = "E-mail inválido")]
        public string Email { get; set; }

        public string Funcao { get; set; }

        [OpcionalMinLength(8, ErrorMessage = "Senha deve ter no mínimo 8 caracteres")]
        public string Password { get; set; }

        public string Token { get; set; }
    }
}