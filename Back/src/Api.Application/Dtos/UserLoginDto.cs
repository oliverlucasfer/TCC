using System.ComponentModel.DataAnnotations;

namespace Api.Application.Dtos
{
    public class UserLoginDto
    {
        [Required(ErrorMessage = "Usuário é obrigatório")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Senha é obrigatória")]
        public string Password { get; set; }
    }
}