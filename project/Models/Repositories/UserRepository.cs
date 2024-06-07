using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace project.Models.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationContext _db;

        public UserRepository(ApplicationContext db)
        {
            _db = db;
        }

        public async Task<IdentityUser> GetUserByNameAsync(string name)
        {
            return await _db.Users.FirstOrDefaultAsync(u => u.UserName == name);
        }

        public async Task<IdentityUser> GetUserByEmailAsync(string email)
        {
            return await _db.Users.FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<List<IdentityUser>> GetAllAsync()
        {   
            List<IdentityUser> users = await _db.Set<IdentityUser>().ToListAsync();
            return users;
        }

        public async Task RegisterAsync(IdentityUser user)
        {
            await _db.Set<IdentityUser>().AddAsync(user);
            await SaveAsync();
        }

        public async Task BlockUserAsync(string id)
        {
            IdentityUser user = await _db.Users.FirstOrDefaultAsync(u => u.Id == id);
            user.LockoutEnabled = true;
            user.LockoutEnd = DateTime.Now.AddYears(1);
            await SaveAsync();
        }

        public async Task UnblockUserAsync(string id)
        {
            IdentityUser user = await _db.Users.FirstOrDefaultAsync(u => u.Id == id);
            user.LockoutEnabled = false;
            user.LockoutEnd = DateTime.Now;
            await SaveAsync();
        }

        public async Task DeleteUserAsync(string id)
        {
            IdentityUser user = await _db.Users.FirstOrDefaultAsync(u => u.Id == id);
            _db.Users.Remove(user);
            await SaveAsync();
        }

        public async Task PromoteUserToAdminAsync(string id)
        {
            IdentityUser user = await _db.Users.FirstOrDefaultAsync(u => u.Id == id);
            await SaveAsync();
        }

        public async Task<IdentityUser> AuthenticateAsync(string email, string password)
        {
            IdentityUser user = await _db.Users.FirstOrDefaultAsync(u =>
                u.Email == email && u.PasswordHash == password);
            
            return user;
        }

        public async Task SaveAsync()
        {
            await _db.SaveChangesAsync();
        }
    }
}