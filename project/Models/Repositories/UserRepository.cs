using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using project.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace project.Models.Repositories
{
    public class UserRepository
    {
        private readonly MyDbContext _context;

        public UserRepository(MyDbContext context)
        {
            _context = context;
        }

        public async Task<User> GetUserByEmail(string email)
        {
            User user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
            return user;
        }

        public async Task<List<User>> GetAllAsync()
        {
            return await _context.Set<User>().ToListAsync();
        }

        public async Task Register(User user)
        {
            _context.Set<User>().Add(user);
            
            await Save();
        }

        public async Task<User> Authenticate(string email, string password)
        {
            User user = await _context.Users.FirstOrDefaultAsync(u =>
                u.Email == email && u.PasswordHash == password);

            return user;
        }

        private async Task Save()
        {
            await _context.SaveChangesAsync();
        }
    }
}
