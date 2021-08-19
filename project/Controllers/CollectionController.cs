using Microsoft.AspNetCore.Mvc;
using project.Models.Entities;
using project.Models.Repositories;
using System.Threading.Tasks;

namespace project.Controllers
{
    
    public class CollectionController : Controller
    {
        private readonly CollectionRepository _collectionRepository;
        private readonly UserRepository _userRepository;

        public CollectionController(
            CollectionRepository collectionRepository,
            UserRepository userRepository)
        {
            _collectionRepository = collectionRepository;
            _userRepository = userRepository;
        }

        [HttpGet]
        public async Task<IActionResult> Collection(int id)
        {
            Collection collection = await _collectionRepository.GetByIdAsync(id);
            User user = await _userRepository.GetUserByEmailAsync(User.Identity.Name);
            ViewData["User"] = user;
            return View(collection);
        }

        public async Task<IActionResult> SortById(int id) {
            return View("Collection", new { id = id});
        }

        public async Task<IActionResult> SortByName(int id)
        {
            return View("Collection", new { id = id });
        }
    }
}
