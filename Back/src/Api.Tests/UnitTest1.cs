using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Moq;
using Xunit;
using Api.Domain.Identity;
using Api.Application;
using Api.Domain.Enums;
using Api.Application.Contratos;
using Api.Application.Dtos;
using Microsoft.Extensions.Options;
using Api.Application.Models;
using System.Security.Claims;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;

namespace Api.Tests
{
    public class TokenServiceTests
    {
        [Fact]
        public async Task CreateToken_Deve_Incluir_Role_Issuer_Audience_E_Algoritmo_HS512()
        {
            var options = Options.Create(new JwtOptions
            {
                TokenKey = "minha-chave-jwt-forte-1234567890-abcdefghijklmnopqrstuvwxyz-ABCDEFGHIJKLMNOPQRSTUVWXYZ-64bytes",
                Issuer = "ProDocs",
                Audience = "ProDocsClient",
                ExpirationHours = 12
            });

            var service = new TokenService(options);
            var user = new User { Id = 1, UserName = "alice", Tipo = Tipo.Administrador };

            var token = await service.CreateToken(user);

            var handler = new JwtSecurityTokenHandler();
            var jwt = handler.ReadJwtToken(token);

            Assert.Equal("HS512", jwt.Header.Alg);
            Assert.Equal("ProDocs", jwt.Issuer);
            Assert.Equal("ProDocsClient", jwt.Audiences.First());
            Assert.Contains(jwt.Claims, c => c.Type == "role" && c.Value == "Administrador");
            Assert.Contains(jwt.Claims, c => c.Type == "unique_name" && c.Value == "alice");
        }

        [Fact]
        public void TokenService_Com_Chave_Curta_Deve_Lancar()
        {
            var options = Options.Create(new JwtOptions { TokenKey = "curta" });

            Assert.Throws<System.InvalidOperationException>(() => new TokenService(options));
        }
    }

    public class AccountServiceTests
    {
        private static UserManager<User> CriarUserManager()
        {
            var store = new Mock<IUserStore<User>>();
            store.As<Microsoft.AspNetCore.Identity.IUserPasswordStore<User>>();
            store.Setup(s => s.CreateAsync(It.IsAny<User>(), It.IsAny<System.Threading.CancellationToken>()))
                 .ReturnsAsync(IdentityResult.Success)
                 .Callback<User, System.Threading.CancellationToken>((user, _) => { });
            var userManager = new UserManager<User>(
                store.Object,
                null,
                new PasswordHasher<User>(),
                Array.Empty<IUserValidator<User>>(),
                Array.Empty<IPasswordValidator<User>>(),
                new UpperInvariantLookupNormalizer(),
                new IdentityErrorDescriber(),
                null,
                null);
            return userManager;
        }

        [Fact]
        public async Task CreateAccountAsync_Deve_Forcar_Tipo_UsuarioComum()
        {
            var userManager = CriarUserManager();
            var userPersistence = new Mock<IUserPersistence>();
            var geralPersistence = new Mock<IGeralPersistence>();

            var service = new AccountService(
                userManager,
                null,
                userPersistence.Object,
                geralPersistence.Object);

            var dto = new UserDto
            {
                UserName = "hacker",
                Email = "h@h.com",
                Password = "senha1234",
                PrimeiroNome = "H",
                Tipo = Tipo.Administrador
            };

            var resultado = await service.CreateAccountAsync(dto);

            Assert.NotNull(resultado);
            Assert.Equal(Tipo.UsuarioComum, resultado.Tipo);
        }
    }

    public class PageParamsTests
    {
        [Fact]
        public void PageNumber_Negativo_Deve_Ser_Clampado_Para_1()
        {
            var p = new PageParams { PageNumber = -5 };
            Assert.Equal(1, p.PageNumber);
        }

        [Fact]
        public void PageSize_Acima_Do_Maximo_Deve_Ser_Limitado()
        {
            var p = new PageParams { PageSize = 1000 };
            Assert.Equal(PageParams.MaxPageSize, p.PageSize);
        }
    }
}