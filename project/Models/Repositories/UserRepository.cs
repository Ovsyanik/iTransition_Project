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

        public async Task<User> GetUserByNameAsync(string name)
        {
            return await _context.Users
                .FirstOrDefaultAsync(u => u.UserName == name);
        }

        public async Task<User> GetUserByEmailAsync(string email)
        {
            return await _context.Users
                .FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<List<User>> GetAllAsync()
        {   
            List<User> users = await _context.Set<User>().ToListAsync();
            return users;
        }

        public async Task RegisterAsync(User user)
        {
            await _context.Set<User>().AddAsync(user);

            await SaveAsync();
        }

        public async Task BlockUserAsync(string id)
        {
            User user = _context.Users.FirstOrDefault(u => u.Id == id);
            user.LockoutEnabled = true;
            user.LockoutEnd = DateTime.Now.AddYears(1);
            await SaveAsync();
        }


        public async Task UnblockUserAsync(string id)
        {
            User user = _context.Users.FirstOrDefault(u => u.Id == id);
            user.LockoutEnabled = false;
            user.LockoutEnd = DateTime.Now;
            await SaveAsync();
        }


        public async Task DeleteUserAsync(string id)
        {
            User user = _context.Users.FirstOrDefault(u => u.Id == id);
            _context.Users.Remove(user);
            await SaveAsync();
        }

        public async Task PromoteUserToAdminAsync(string id)
        {
            User user = _context.Users.FirstOrDefault(u => u.Id == id);
            user.Role = RoleUser.Admin;
            await SaveAsync();
        }

        public async Task<User> AuthenticateAsync(string email, string password)
        {
            User user = await _context.Users.FirstOrDefaultAsync(u =>
                u.Email == email && u.PasswordHash == password);
            
            return user;
        }


        private async Task SaveAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}