namespace Api.Application.Dtos
{
    public class UserUpdateDto
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string PrimeiroNome { get; set; }
        public string UltimoNome { get; set; }
        public string Email { get; set; }
        public string Funcao { get; set; }
        public string Password { get; set; }
        public string Token { get; set; }
    }
}