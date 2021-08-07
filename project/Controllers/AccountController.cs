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
        private readonly IAuthenticationSchemeProvider authenticationSchemeProvider;
        private readonly SignInManager<IdentityUser> signInManager;

        public AccountController(SignInManager<IdentityUser> signInManager, UserRepository userRepository)
        {
            this.signInManager = signInManager;
            this.userRepository = userRepository;
        }

        //[HttpGet]
        //public async Task<IActionResult> Authentication()
        //{
        //    var allSchemeProvider = (await authenticationSchemeProvider.GetAllSchemesAsync())
        //        .Select(n => n.DisplayName).Where(n => !string.IsNullOrEmpty(n));

        //    return View(allSchemeProvider);
        //}

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
            User user = await userRepository.Authenticate(email, password);

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

        //[HttpGet]
        //[AllowAnonymous]
        //public IActionResult ExternalLogin(string provider)
        //{
            
        //    return Challenge(new AuthenticationProperties { RedirectUri = "/" }, provider);
        //}

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
        public async Task<IActionResult> ExternalLoginCallback(string returnUrl = null, string remoteError = null)
        {
            returnUrl = returnUrl ?? Url.Content("~/");
            LoginViewModel loginViewModel = new LoginViewModel
            {
                ReturnUrl = returnUrl,
                ExternalLogins = (await signInManager.GetExternalAuthenticationSchemesAsync()).ToList()
            };

            if (remoteError != null)
            {
                ModelState.AddModelError(string.Empty, $"Error from external provider: {remoteError}");

                return View("Authentication", loginViewModel);
            }

            var info = await signInManager.GetExternalLoginInfoAsync();
            if (info == null)
            {
                ModelState.AddModelError(string.Empty, "Error loading external login information.");

                return View("Login", loginViewModel);
            }

            var signInResult = await signInManager.ExternalLoginSignInAsync(info.LoginProvider,
                                        info.ProviderKey, isPersistent: false, bypassTwoFactor: true);

            if (signInResult.Succeeded)
            {
                return LocalRedirect(returnUrl);
            }
            else
            {
                var email = info.Principal.FindFirstValue(ClaimTypes.Email);

                if (email != null)
                {
                    //var user = await userRepository.GetByEmail(email);

                    //if (user == null)
                    //{
                    //    user = new ApplicationUser
                    //    {
                    //        UserName = info.Principal.FindFirstValue(ClaimTypes.Email),
                    //        Email = info.Principal.FindFirstValue(ClaimTypes.Email)
                    //    };

                    //    await userRepository.CreateAsync(user);
                    //}

                    //await userRepository.AddLoginAsync(user, info);
                    //await signInManager.SignInAsync(user, isPersistent: false);

                    return LocalRedirect(returnUrl);
                }

                ViewBag.ErrorTitle = $"Email claim not received from: {info.LoginProvider}";
                ViewBag.ErrorMessage = "Please contact support on Pragim@PragimTech.com";

                return View("Error");
                }
        }

        public async Task<IActionResult> SignOut()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Authentication", "Account");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RegistrationUser(string firstName, string lastName, string email, string password)
        {
            User user = await userRepository.GetUserByEmail(email);
            if (user == null)
            {
                await userRepository.Register(new User(
                    firstName, lastName, RoleUser.User, email, password));

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
