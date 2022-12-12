using System.Collections.Generic;
using Api.Domain.Enums;
using Microsoft.AspNetCore.Identity;

namespace Api.Domain.Identity
{
    public class User : IdentityUser<int>
    {
        public string PrimeiroNome { get; set; }
        public string UltimoNome { get; set; }
        public Tipo Tipo { get; set; }
        public IEnumerable<UserRole> UserRoles { get; set; }
    }
}