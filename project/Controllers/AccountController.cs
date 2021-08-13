using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Facebook;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using project.Models;
using project.Models.Entities;
using project.Models.Repositories;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace project.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserRepository userRepository;
        private readonly SignInManager<User> signInManager;

        public AccountController(SignInManager<User> signInManager, UserRepository userRepository)
        {
            this.signInManager = signInManager;
            this.userRepository = userRepository;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Authentication(string returnUrl = "~/")
        {
            LoginViewModel model = new LoginViewModel
            {
                ReturnUrl = returnUrl,
                ExternalLogins =
                (await signInManager.GetExternalAuthenticationSchemesAsync()).ToList()
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AuthenticationUser(string email, string password)
        {
            IdentityUser user = await userRepository.Authenticate(email, password);

            if (user != null)
            {
                await Authenticate(user.Email);

                return RedirectToAction("Index", "Home", new { email = email });
            }
            else
            {
                return RedirectToAction("Authentication", "Account");
            }
        }

        [HttpGet]
        public IActionResult Registration()
        {
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        public IActionResult ExternalLogin(string provider, string returnUrl)
        {
                var redirectUrl = Url.Action("ExternalLoginCallback", "Account",
                    new { ReturnUrl = returnUrl });

                var properties =
                    signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);

                return new ChallengeResult(provider, properties);
            
        }


        [AllowAnonymous]
        public IActionResult GoogleLogin()
        {
            string redirectUrl = Url.Action("GoogleResponse", "Account");
            var properties = signInManager.ConfigureExternalAuthenticationProperties("Google", redirectUrl);
            return new ChallengeResult("Google", properties);
        }


        [AllowAnonymous]
        public async Task<IActionResult> GoogleResponse()
        {
            ExternalLoginInfo info = await signInManager.GetExternalLoginInfoAsync();
             
            await Authenticate(info.Principal.FindFirst(ClaimTypes.Email).Value);

            User user = new User
            {
                Email = info.Principal.FindFirst(ClaimTypes.Email).Value,
                UserName = info.Principal.FindFirst(ClaimTypes.Name).Value,
                Provider = info.LoginProvider,
                Role = RoleUser.User
            };

            User checkUser = userRepository.GetUserByEmail(info.Principal.FindFirst(ClaimTypes.Email).Value);

            if (checkUser == null)
            {
                await userRepository.Register(user);
            }

            return RedirectToAction("Index", "Home");
        }


        public async Task<IActionResult> SignOut()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            return RedirectToAction("Authentication", "Account");
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RegistrationUser(string userName, string email, string password)
        {
            User user = userRepository.GetUserByEmail(email);

            if (user == null)
            {
                User newUser = new User
                {
                    UserName = userName,
                    Email = email,
                    PasswordHash = password,
                    Role = RoleUser.User
                };

                await userRepository.Register(newUser);

                return RedirectToAction("Authentication", "Account");
            }
            else
            {
                return RedirectToAction("Registration", "Account");
            }
        }


        private async Task Authenticate(string email)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimsIdentity.DefaultNameClaimType, email)
            };

            ClaimsIdentity id = new ClaimsIdentity(claims, "ApplicationCookie",
                ClaimsIdentity.DefaultNameClaimType, ClaimsIdentity.DefaultRoleClaimType);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(id));
        }
    }
}