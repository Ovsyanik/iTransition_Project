using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using project.Models.Repositories;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace project.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public AdminController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }


        public async Task<IActionResult> Admin()
        {
            List<IdentityUser> users = await _unitOfWork.Users.GetAllAsync();
            return View(users);
        }

        public async Task<IActionResult> BlockUser(string id)
        {
            await _unitOfWork.Users.BlockUserAsync(id);
            return RedirectToAction("Admin");
        }

        public async Task<IActionResult> UnblockUser(string id)
        {
            await _unitOfWork.Users.UnblockUserAsync(id);
            return RedirectToAction("Admin");
        }

        public async Task<IActionResult> DeleteUser(string id)
        {
            await _unitOfWork.Users.UnblockUserAsync(id);
            return RedirectToAction("Admin");
        }

        public async Task<IActionResult> PromoteUserToAdmin(string id)
        {
            await _unitOfWork.Users.PromoteUserToAdminAsync(id);
            return RedirectToAction("Admin");
        }

        public async Task<IActionResult> UserCollections(string email)
        {
            IdentityUser user = await _unitOfWork.Users.GetUserByEmailAsync(email);
            var collections = _unitOfWork.Collections.GetAllByUserAsync(user);

            ViewData["User"] = user;
            return View(collections);
        }
    }
}
