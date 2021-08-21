using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using project.Models.Entities;
using project.Models.Repositories;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace project.Controllers
{
    [Authorize]
    public class AdminController : Controller
    {
        private readonly UserRepository _userRepository;
        private readonly CollectionRepository _collectionRepository;

        public AdminController(
            UserRepository userRepository, 
            CollectionRepository collectionRepository)
        {
            _userRepository = userRepository;
            _collectionRepository = collectionRepository;
        }


        public async Task<IActionResult> Admin()
        {
            List<User> users = await _userRepository.GetAllAsync();
            return View(users);
        }

        public async Task<IActionResult> BlockUser(string id)
        {
            await _userRepository.BlockUserAsync(id);
            return RedirectToAction("Admin");
        }

        public async Task<IActionResult> UnblockUser(string id)
        {
            await _userRepository.UnblockUserAsync(id);
            return RedirectToAction("Admin");
        }

        public async Task<IActionResult> DeleteUser(string id)
        {
            await _userRepository.UnblockUserAsync(id);
            return RedirectToAction("Admin");
        }

        public async Task<IActionResult> PromoteUserToAdmin(string id)
        {
            await _userRepository.PromoteUserToAdminAsync(id);
            return RedirectToAction("Admin");
        }


        public async Task<IActionResult> UserCollections(string email)
        {
            User user = await _userRepository.GetUserByEmailAsync(email);
            var collections = _collectionRepository.GetAllByUser(user);

            ViewData["User"] = user;
            return View(collections);
        }
    }
}
