using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Api.Application.Contratos;
using Api.Application.Dtos;
using Api.Application.Helpers;
using Api.Domain.Identity;
using Api.Domain.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Api.Application
{
    public class AccountService : IAccountService
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IUserPersistence _userPersistence;
        private readonly IGeralPersistence _geralPersistence;
        public AccountService(UserManager<User> userManager, SignInManager<User> signInManager, IUserPersistence userPersistence, IGeralPersistence geralPersistence)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _userPersistence = userPersistence;
            _geralPersistence = geralPersistence;
        }
        public async Task<SignInResult> CheckUserPasswordAsync(User user, string password)
        {
            try
            {
                if (user == null) return SignInResult.Failed;

                return await _signInManager.CheckPasswordSignInAsync(user, password, false);
            }
            catch (System.Exception ex)
            {

                throw new Exception($"Erro ao tentar verificar password. Error: {ex.Message}");
            }
        }

        public async Task<UserReturnDto> CreateAccountAsync(UserDto userDto)
        {
            try
            {
                userDto.Tipo = Tipo.UsuarioComum;
                var user = userDto.ToEntity();
                var result = await _userManager.CreateAsync(user, userDto.Password);

                if (result.Succeeded)
                {
                    var userToReturn = user.ToReturnDto();
                    return userToReturn;
                }

                return null;
            }
            catch (System.Exception ex)
            {

                throw new Exception($"Erro ao tentar pegar Usuário. Error: {ex.Message}");
            }
        }

        public async Task<User> GetUserByUserNameAsync(string username)
        {
            try
            {
                var user = await _userPersistence.GetUserByUserNameAsync(username);
                return user;
            }
            catch (System.Exception ex)
            {

                throw new Exception($"Erro ao tentar pegar Usuário. Error: {ex.Message}");
            }
        }

        public async Task<UserUpdateDto> UpdateAccount(UserUpdateDto userUpdateDto)
        {
            try
            {
                var user = await _userPersistence.GetUserByUserNameAsync(userUpdateDto.UserName);
                if (user == null) return null;

                user.UpdateFrom(userUpdateDto);

                if (!string.IsNullOrWhiteSpace(userUpdateDto.Password))
                {
                    var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                    var result = await _userManager.ResetPasswordAsync(user, token, userUpdateDto.Password);
                    if (!result.Succeeded)
                        return null;
                }

                _geralPersistence.Update<User>(user);

                if (await _geralPersistence.SaveChangesAsync())
                {
                    var userRetorno = await _userPersistence.GetUserByUserNameAsync(user.UserName);

                    return userRetorno.ToUpdateDto();
                }

                return null;
            }
            catch (System.Exception ex)
            {

                throw new Exception($"Erro ao tentar atualizar Usuário. Error: {ex.Message}");
            }
        }

        public async Task<bool> UserExists(string userName)
        {
            try
            {
                return await _userManager.Users.AnyAsync(user => user.UserName == userName.ToLower());
            }
            catch (System.Exception ex)
            {

                throw new Exception($"Erro ao tentar verificar existência. Error: {ex.Message}");
            }
        }
    }
}