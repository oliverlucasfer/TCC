using System.Security.Claims;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Api.Application.Contratos;
using Api.Application.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Api.Extensions;
using Api.Persistence.Contexto;
using Microsoft.EntityFrameworkCore;

namespace Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AccountController : ControllerBase
    {
        private readonly IAccountService _accountService;
        private readonly ITokenService _tokenService;
        private readonly ApiContext _context;
        public AccountController(IAccountService accountService, ITokenService tokenService, ApiContext context)
        {
            _tokenService = tokenService;
            _accountService = accountService;
            _context = context;

        }

        [HttpGet("GetUser")]

        public async Task<IActionResult> GetUser()
        {
            try
            {
                var user = await _context.Users.ToListAsync();
                return Ok(user);
            }
            catch (System.Exception ex)
            {

                return this.StatusCode(
                    StatusCodes.Status500InternalServerError,
                    $"Erro ao tentar recuparar Usuário. Erro: {ex.Message}");
            }
        }

        [HttpPost("Register")]

        public async Task<IActionResult> Register(UserDto userDto)
        {
            try
            {
                if (await _accountService.UserExists(userDto.UserName)) return BadRequest("Usuário Existente");

                var user = await _accountService.CreateAccountAsync(userDto);
                if (user != null) return Ok(user);

                return BadRequest("Usuário não criado");
            }
            catch (System.Exception ex)
            {

                return this.StatusCode(
                    StatusCodes.Status500InternalServerError,
                    $"Erro ao tentar Registrar Usuário. Erro: {ex.Message}");
            }
        }


        [HttpPost("Login")]

        public async Task<IActionResult> Login(UserLoginDto userLoginDto)
        {
            try
            {
                var user = await _accountService.GetUserByUserNameAsync(userLoginDto.Username);
                if (user == null) return Unauthorized("Usuário ou Senha está errado");

                var result = await _accountService.CheckUserPasswordAsync(user, userLoginDto.Password);
                if (!result.Succeeded) return Unauthorized();

                return Ok(new
                {
                    userName = user.UserName,
                    primeiroNome = user.PrimeiroNome,
                    token = _tokenService.CreateToken(user)
                });
            }
            catch (System.Exception ex)
            {

                return this.StatusCode(
                    StatusCodes.Status500InternalServerError,
                    $"Erro ao tentar recuparar Usuário. Erro: {ex.Message}");
            }
        }

        [HttpPost("UpdateUser")]

        public async Task<IActionResult> Update(UserUpdateDto userUpdateDto)
        {
            try
            {
                var user = await _accountService.GetUserByUserNameAsync(User.GetUserName());
                if (user == null) return Unauthorized("Usuário Inválido");

                var userReturn = await _accountService.UpdateAccount(userUpdateDto);
                if (userReturn == null) return NoContent();

                return Ok(userReturn);
            }
            catch (System.Exception ex)
            {

                return this.StatusCode(
                    StatusCodes.Status500InternalServerError,
                    $"Erro ao tentar recuparar Usuário. Erro: {ex.Message}");
            }
        }
    }
}