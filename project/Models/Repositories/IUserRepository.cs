using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System;
using System.Threading.Tasks;

namespace project.Models.Repositories
{
    public interface IUserRepository
    {
        Task<IdentityUser> GetUserByNameAsync(string name);
        Task<IdentityUser> GetUserByEmailAsync(string email);
        Task<List<IdentityUser>> GetAllAsync();
        Task RegisterAsync(IdentityUser user);
        Task BlockUserAsync(string id);
        Task UnblockUserAsync(string id);
        Task DeleteUserAsync(string id);
        Task PromoteUserToAdminAsync(string id);
        Task<IdentityUser> AuthenticateAsync(string email, string password);
        Task SaveAsync();
    }
}