using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Api.Domain.Identity;
using Api.Persistence.Contexto;
using Api.Persistence.Contratos;
using Microsoft.EntityFrameworkCore;

namespace Api.Persistence
{
    public class UserPersistence : GeralPersistence, IUserPersistence
    {
        private readonly ApiContext _context;
        public UserPersistence(ApiContext context) : base(context)
        {
            _context = context;

        }

        public async Task<IEnumerable<User>> GetUsersAsync()
        {
            return await _context.Users.ToListAsync();
        }
        public async Task<User> GetUserByIdAsync(int id)
        {
            return await _context.Users.FindAsync(id);
        }

        public async Task<User> GetUserByUserNameAsync(string userName)
        {
            return await _context.Users.SingleOrDefaultAsync(x => x.UserName == userName.ToLower());
        }

    }
}