using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Api.Application.Contratos;
using Api.Application.Models;
using Api.Domain.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Api.Application
{
    public class TokenService : ITokenService
    {
        private readonly JwtOptions _options;
        private readonly SymmetricSecurityKey _key;

        public TokenService(IOptions<JwtOptions> options)
        {
            _options = options.Value;
            if (string.IsNullOrEmpty(_options.TokenKey))
                throw new InvalidOperationException(
                    "TokenKey não configurada. Em Development use 'dotnet user-secrets set \"TokenKey\" \"<chave>\"'; em produção, defina a variável de ambiente TokenKey.");
            if (Encoding.UTF8.GetBytes(_options.TokenKey).Length < 64)
                throw new InvalidOperationException(
                    "TokenKey muito curta. HS512 exige chave de pelo menos 64 bytes. Use: openssl rand -base64 48");
            _key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.TokenKey));
        }

        public Task<string> CreateToken(User user)
        {
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.NameId, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.UniqueName, user.UserName),
                new Claim(ClaimTypes.Role, user.Tipo.ToString())
            };

            var creds = new SigningCredentials(_key, SecurityAlgorithms.HmacSha512);

            var tokenDescription = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Issuer = _options.Issuer,
                Audience = _options.Audience,
                Expires = DateTime.UtcNow.AddHours(_options.ExpirationHours),
                SigningCredentials = creds
            };

            var tokenHandler = new JwtSecurityTokenHandler();

            var token = tokenHandler.CreateToken(tokenDescription);

            return Task.FromResult(tokenHandler.WriteToken(token));
        }
    }
}
