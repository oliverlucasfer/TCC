using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Api.Domain.Identity;

namespace Api.Application.Contratos
{
    public interface ITokenService
    {
        Task<string> CreateToken(User user);
    }
}