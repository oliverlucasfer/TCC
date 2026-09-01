using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Api.Application.Dtos;
using Api.Domain.Identity;
using Microsoft.AspNetCore.Identity;

namespace Api.Application.Contratos
{
    public interface IAccountService
    {
        Task<bool> UserExists(string username);
        Task<User> GetUserByUserNameAsync(string username);
        Task<SignInResult> CheckUserPasswordAsync(User user, string password);
        Task<UserReturnDto> CreateAccountAsync(UserDto userDto);
        Task<UserUpdateDto> UpdateAccount(UserUpdateDto userUpdateDto);
    }
}