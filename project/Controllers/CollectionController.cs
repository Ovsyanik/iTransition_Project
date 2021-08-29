using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using project.Models.Entities;
using project.Models.Repositories;
using System.Linq;
using System.Threading.Tasks;

namespace project.Controllers
{

    [Authorize]
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

        [AllowAnonymous]
        public async Task<IActionResult> Collections(string email)
        {
            User user = await _userRepository.GetUserByEmailAsync(email ?? User.Identity.Name);
            var collections = _collectionRepository.GetAllByUser(user);
            ViewData["UserEmail"] = email;
            return View("Collections", collections);
        }


        [AllowAnonymous]
        public async Task<IActionResult> Collection(int id)
        {
            Collection collection = await _collectionRepository.GetByIdAsync(id);
            return View(collection);
        }


        [HttpPost]
        public async Task<IActionResult> Filter(int collectionId, string field, string text)
        {
            Collection collection = null;
            if(field.Equals("Name"))
            {
                collection = await _collectionRepository.FilterByName(collectionId, text);
            } 
            else if(field.Equals("Tags"))
            {
                collection = await _collectionRepository.FilterByTags(collectionId, text);
            }
            return View("Collection", collection);
        }


        public IActionResult AddCollection(string userEmail)
        {
            ViewData["UserEmail"] = userEmail;
            return View();
        }


        public async Task<IActionResult> EditCollection(string userEmail, int id)
        {
            Collection collection = await _collectionRepository.GetByIdAsync(id);
            ViewData["UserEmail"] = userEmail;
            return View("AddCollection", collection);
        }


        [HttpPost]
        public async Task<IActionResult> AddCollectionPost(string userEmail, IFormFile files, Collection newCollection, string[] field, CustomFieldType[] newFieldType)
        {
            int idCollection;
            if (files != null)
            {
                idCollection = await _collectionRepository.AddAsync(new Collection
                {
                    Name = newCollection.Name,
                    Description = newCollection.Description,
                    Type = newCollection.Type,
                    PathImage = _cloudRepository.UploadPhoto(files.OpenReadStream()),
                    User = await _userRepository.GetUserByEmailAsync(userEmail ?? User.Identity.Name)
                });
            }
            else
            {
                idCollection = await _collectionRepository.AddAsync(new Collection
                {
                    Name = newCollection.Name,
                    Description = newCollection.Description,
                    Type = newCollection.Type,
                    User = await _userRepository.GetUserByEmailAsync(userEmail ?? User.Identity.Name)
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
        public async Task<IActionResult> EditCollection(Collection collection, IFormFile files, int[] fieldId, string[] field, CustomFieldType[] newFieldType)
        {
            if(files != null)
            {
                collection.PathImage = _cloudRepository.UploadPhoto(files.OpenReadStream());
            }

            for (int i = 0; i < field.Length; i++)
            {
                if (fieldId != null )
                {
                    if (fieldId.Length != 0)
                    {
                        await _customFieldRepository.EditCustomFieldAsync(fieldId[i], field[i], newFieldType[i]);
                        fieldId = fieldId.Where((source, index) => index != i).ToArray();
                        field = field.Where((source, index) => index != i).ToArray();
                        newFieldType = newFieldType.Where((source, index) => index != i).ToArray();
                        i--;
                    }
                    else
                    {
                        await _customFieldRepository.AddAsync(new CustomField
                        {
                            Title = field[i],
                            CollectionId = collection.Id,
                            CustomFieldType = newFieldType[i]
                        });
                    }
                } 
            }
            collection = await _collectionRepository.EditCollection(collection);
            return RedirectToAction("Collections", new { email = collection.User.Email});
        }

        [AllowAnonymous]
        public async Task<IActionResult> SortById(int id) {
            Collection collection = await _collectionRepository.SortByIdAsync(id);
            return View("Collection", collection);
        }


        [AllowAnonymous]
        public async Task<IActionResult> SortByName(int id)
        {
            Collection collection = await _collectionRepository.SortByNameAsync(id);
            return View("Collection", collection);
        }
    }
}