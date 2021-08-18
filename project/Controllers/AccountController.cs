using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using project.Models.Entities;
using project.Models.Repositories;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace project.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserRepository _userRepository;
        private readonly SignInManager<User> _signInManager;

        public AccountController(
            SignInManager<User> signInManager, 
            UserRepository userRepository)
        {
            _signInManager = signInManager;
            _userRepository = userRepository;
        }


        [HttpGet]
        [AllowAnonymous]
        public IActionResult Authentication()
        {
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AuthenticationUser(string email, string password)
        {
            IdentityUser user = await _userRepository.AuthenticateAsync(email, password);

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
        public IActionResult GoogleLogin()
        {
            string redirectUrl = Url.Action("ExternalResponse", "Account");
            var properties = _signInManager.ConfigureExternalAuthenticationProperties("Google", redirectUrl);
            return new ChallengeResult("Google", properties);
        }


        [AllowAnonymous]
        public IActionResult FacebookLogin()
        {
            string redirectUrl = Url.Action("ExternalResponse", "Account");
            var properties = _signInManager.ConfigureExternalAuthenticationProperties("Facebook", redirectUrl);
            return new ChallengeResult("Facebook", properties);
        }


        [AllowAnonymous]
        public async Task<IActionResult> ExternalResponse()
        {
            ExternalLoginInfo info = await _signInManager.GetExternalLoginInfoAsync();
            if (info.LoginProvider == "Facebook")
                await Authenticate(info.Principal.FindFirstValue(ClaimTypes.NameIdentifier));
            else
                await Authenticate(info.Principal.FindFirstValue(ClaimTypes.Email));

            User user, checkUser;

            if (info.LoginProvider == "Google")
            {
                user = new User
                {
                    Email = info.Principal.FindFirst(ClaimTypes.Email).Value,
                    UserName = info.Principal.FindFirst(ClaimTypes.Name).Value,
                    Provider = info.LoginProvider,
                    Role = RoleUser.User
                };

                checkUser = await _userRepository.GetUserByEmailAsync(info.Principal.FindFirst(ClaimTypes.Email).Value);
            } else
            {
                user = new User
                {
                    Email = info.Principal.FindFirst(ClaimTypes.NameIdentifier).Value,
                    UserName = info.Principal.FindFirst(ClaimTypes.Name).Value,
                    Provider = info.LoginProvider,
                    Role = RoleUser.User
                };

                checkUser = await _userRepository.GetUserByEmailAsync(info.Principal.FindFirst(ClaimTypes.NameIdentifier).Value);
            }
            if (checkUser == null)
            {
                await _userRepository.RegisterAsync(user);
            }

            return RedirectToAction("Index", "Home");
        }


        public async Task<IActionResult> SignOut()
        {
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);
            
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            return RedirectToAction("Authentication", "Account");
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RegistrationUser(string userName, string email, string password)
        {
            User user = await _userRepository.GetUserByEmailAsync(email);

            if (user == null)
            {
                User newUser = new User
                {
                    UserName = userName,
                    Email = email,
                    PasswordHash = password,
                    Role = RoleUser.User
                };

                await _userRepository.RegisterAsync(newUser);

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