using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using project.Models.Entities;
using project.Models.Repositories;
using System.Threading.Tasks;

namespace project.Controllers
{
    
    public class CollectionController : Controller
    {
        private readonly CollectionRepository _collectionRepository;
        private readonly CloudRepository _cloudRepository;
        private readonly UserRepository _userRepository;
        private readonly CustomFieldRepository _customFieldRepository;

        public CollectionController(
            CollectionRepository collectionRepository,
            CloudRepository cloudRepository,
            CustomFieldRepository customFieldRepository,
            UserRepository userRepository)
        {
            _collectionRepository = collectionRepository;
            _userRepository = userRepository;
            _cloudRepository = cloudRepository;
            _customFieldRepository = customFieldRepository;
        }


        public async Task<IActionResult> Collections(string email)
        {
            User user = await _userRepository.GetUserByEmailAsync(email ?? User.Identity.Name);
            var collections = _collectionRepository.GetAllByUser(user);

            return View("Collections", collections);
        }


        public async Task<IActionResult> Collection(int id)
        {
            Collection collection = await _collectionRepository.GetByIdAsync(id);
            return View(collection);
        }


        public IActionResult AddCollection()
        {
            return View();
        }


        public async Task<IActionResult> EditCollection(int id)
        {
            Collection collection = await _collectionRepository.GetByIdAsync(id);
            return View("AddCollection", collection);
        }


        [HttpPost]
        public async Task<IActionResult> AddCollectionPost(IFormFile files, string description, string name, TypeItem theme, string[] field, CustomFieldType[] newFieldType)
        {
            int idCollection;
            if (files != null)
            {
                idCollection = await _collectionRepository.AddAsync(new Collection
                {
                    Name = name,
                    Description = description,
                    Type = theme,
                    PathImage = _cloudRepository.UploadPhoto(files.OpenReadStream()),
                    User = await _userRepository.GetUserByEmailAsync(User.Identity.Name)
                });
            }
            else
            {
                idCollection = await _collectionRepository.AddAsync(new Collection
                {
                    Name = name,
                    Description = description,
                    Type = theme,
                    User = await _userRepository.GetUserByEmailAsync(User.Identity.Name)
                });
            }

            for (int i = 0; i < field.Length; i++)
            {
                await _customFieldRepository.AddAsync(new CustomField
                {
                    CollectionId = idCollection,
                    Title = field[i],
                    CustomFieldType = newFieldType[i]
                });
            }

            return RedirectToAction("Collections");
        }


        public async Task<IActionResult> DeleteCollection(int id, string userEmail)
        {
            await _collectionRepository.DeleteAsync(id);
            return RedirectToAction("Collections", new { email = userEmail });
        }

        [HttpPost]
        public async Task<IActionResult> EditCollection(IFormFile files, string description, string name, TypeItem theme, string[] field, CustomFieldType[] newFieldType)
        {
            return RedirectToAction("Collections");
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
