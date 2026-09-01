using System.Security.Claims;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Api.Application.Contratos;
using Api.Application.Dtos;
using Api.Application.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Api.Extensions;

namespace Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AccountController : ControllerBase
    {
        private readonly IAccountService _accountService;
        private readonly ITokenService _tokenService;

        public AccountController(IAccountService accountService, ITokenService tokenService)
        {
            _tokenService = tokenService;
            _accountService = accountService;
        }

        [Authorize]
        [HttpGet("GetUser")]
        public async Task<IActionResult> GetUser()
        {
            var user = await _accountService.GetUserByUserNameAsync(User.GetUserName());
            if (user == null) return Unauthorized("Usuário Inválido");

            return Ok(new
            {
                id = user.Id,
                userName = user.UserName,
                email = user.Email,
                primeiroNome = user.PrimeiroNome,
                ultimoNome = user.UltimoNome,
                tipo = user.Tipo.ToString()
            });
        }

        [HttpPost("Register")]
        public async Task<IActionResult> Register(UserDto userDto)
        {
            if (await _accountService.UserExists(userDto.UserName)) return BadRequest("Usuário Existente");

            var user = await _accountService.CreateAccountAsync(userDto);
            if (user != null) return Ok(user);

            return BadRequest("Usuário não criado");
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login(UserLoginDto userLoginDto)
        {
            var user = await _accountService.GetUserByUserNameAsync(userLoginDto.Username);
            if (user == null) return Unauthorized("Usuário ou Senha está errado");

            var result = await _accountService.CheckUserPasswordAsync(user, userLoginDto.Password);
            if (!result.Succeeded) return Unauthorized();

            return Ok(new
            {
                userName = user.UserName,
                primeiroNome = user.PrimeiroNome,
                tipo = user.Tipo.ToString(),
                token = await _tokenService.CreateToken(user)
            });
        }

        [Authorize]
        [HttpPost("UpdateUser")]
        public async Task<IActionResult> Update(UserUpdateDto userUpdateDto)
        {
            userUpdateDto.UserName = User.GetUserName();

            var userReturn = await _accountService.UpdateAccount(userUpdateDto);
            if (userReturn == null) return NoContent();

            return Ok(userReturn);
        }
    }
}