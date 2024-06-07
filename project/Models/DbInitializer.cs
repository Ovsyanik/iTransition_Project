using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;
using System;
using Microsoft.Extensions.DependencyInjection;

namespace project.Models
{
    public static class DbInitializer
    {
        public static async Task Initialize(IServiceProvider serviceProvider)
        {
            var _roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var _userManager = serviceProvider.GetRequiredService<UserManager<IdentityUser>>();
            var _passwordHasher = serviceProvider.GetRequiredService<IPasswordHasher<IdentityUser>>();

            string adminName = "Admin";
            string adminEmail = "Admin@mail.ru";
            string password = "Admin123";

            if (await _roleManager.FindByNameAsync("Admin") == null)
            {
                await _roleManager.CreateAsync(new IdentityRole("Admin"));
            }

            if (await _roleManager.FindByNameAsync("User") == null)
            {
                await _roleManager.CreateAsync(new IdentityRole("User"));
            }

            if (await _userManager.FindByNameAsync(adminEmail) == null)
            {
                string passwordHash = _passwordHasher.HashPassword(new IdentityUser() { Email = adminEmail }, password);

                IdentityUser admin = new() 
                { 
                    Email = adminEmail,
                    PasswordHash = passwordHash,
                    UserName = adminName
                };

                var result = await _userManager.CreateAsync(admin);

                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(admin, "Admin");
                }
            }
        }
    }
}
