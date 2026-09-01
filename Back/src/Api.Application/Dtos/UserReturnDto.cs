using Api.Domain.Enums;

namespace Api.Application.Dtos
{
    public class UserReturnDto
    {
        public string UserName { get; set; }
        public string Email { get; set; }
        public string PrimeiroNome { get; set; }
        public string UltimoNome { get; set; }
        public Tipo Tipo { get; set; }
    }
}