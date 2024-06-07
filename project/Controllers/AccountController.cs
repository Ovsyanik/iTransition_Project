using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System.Security.Claims;
using System.Collections.Generic;
using project.ViewModels;
using System.Linq;

namespace project.Controllers
{
    public class AccountController : Controller
    {
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IPasswordHasher<IdentityUser> _passwordHasher;

        public AccountController(
            SignInManager<IdentityUser> signInManager, 
            UserManager<IdentityUser> userManager,
            IPasswordHasher<IdentityUser> passwordHasher)  
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _passwordHasher = passwordHasher;
        }


        [HttpGet]
        [AllowAnonymous]
        public IActionResult Authentication() => View();


        [HttpGet]
        public IActionResult Registration() => View();


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AuthenticationUser(LoginViewModel model)
        {
            IdentityUser user = await _userManager.FindByEmailAsync(model.Email);

            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "User don't found with of this email, Input correct user data");
                return RedirectToAction("Authentication", "Account");
            }
            else
            {
                var result = await _signInManager.PasswordSignInAsync(user.UserName, model.Password, true, false);

                if (result.Succeeded)
                {
                    string role = _userManager.GetRolesAsync(user).Result.First();
                    await Authenticate(model.Email, role);
                    return RedirectToAction("Index", "Home");
                }
                
                return RedirectToAction("Authentication", "Account");
            }
        }


        [AllowAnonymous]
        public IActionResult ExternalLogin(string provider)
        {
            string redirectUrl = Url.Action("ExternalResponse", "Account");
            var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
            return new ChallengeResult(provider, properties);
        }


        [AllowAnonymous]
        public async Task<IActionResult> ExternalResponse()
        {
            ExternalLoginInfo info = await _signInManager.GetExternalLoginInfoAsync();

            if(info != null) 
            {
                IdentityUser user = await _userManager.FindByEmailAsync(info.Principal.FindFirst(ClaimTypes.Email).Value);
                string role = _userManager.GetRolesAsync(user).Result.First();
                
                await Authenticate(user.Email, role);
            }

            return RedirectToAction("Index", "Home");
        }

        public async Task<IActionResult> LogOut()
        {
            await _signInManager.SignOutAsync();
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            return RedirectToAction("Authentication", "Account");
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Registration(RegisterViewModel model)
        {   
            if(ModelState.IsValid)
            {
                string passwordHash = _passwordHasher.HashPassword(new IdentityUser() { Email = model.Email }, model.Password);

                IdentityUser user = new()
                {
                    UserName = model.UserName,
                    Email = model.Email, 
                    PasswordHash = passwordHash, 
                };

                var result = await _userManager.CreateAsync(user);

                if (result.Succeeded)
                {
                    await _userManager.ConfirmEmailAsync(user, model.Password);
                    await _userManager.AddToRoleAsync(user, "User");

                    return RedirectToAction("Authentication", "Account");
                }
            }

            return View(model);
        }


        private async Task Authenticate(string email, string role)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimsIdentity.DefaultNameClaimType, email),
                new Claim(ClaimsIdentity.DefaultRoleClaimType, role)
            };

            ClaimsIdentity id = new ClaimsIdentity(claims, "ApplicationCookie",
                ClaimsIdentity.DefaultNameClaimType, ClaimsIdentity.DefaultRoleClaimType);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(id));
        }
    }
}