using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using project.Models;
using project.Models.Entities;
using project.Models.Repositories;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using System.Text;
using Microsoft.AspNetCore.Localization;

namespace project.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ItemRepository _itemRepository;
        private readonly UserRepository _userRepository;
        private readonly CloudRepository _googleRepository;
        private readonly TagRepository _tagRepository;
        private readonly CollectionRepository _collectionRepository;
        private readonly CustomFieldRepository _customFieldRepository;

        public HomeController(
            TagRepository tagRepository,
            ItemRepository itemReposetory, 
            UserRepository userRepository, 
            CloudRepository googleRepository, 
            CollectionRepository collectionRepository, 
            CustomFieldRepository customFieldRepository)
        {
            _tagRepository = tagRepository;
            _itemRepository = itemReposetory;
            _userRepository = userRepository;
            _googleRepository = googleRepository;
            _collectionRepository = collectionRepository;
            _customFieldRepository = customFieldRepository;
        }


        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Index()
        {
            ViewData["LastItems"] = await _itemRepository.GetLastItemsAsync();
            User user = await _userRepository.GetUserByEmailAsync(User.Identity.Name);
            var collections = await _collectionRepository.GetCollectionsLargestItem();
            ViewData["CollectionLargestItems"] = collections;
            ViewData["User"] = user;
            return View();
        }


        [HttpGet]
        public async Task<IActionResult> Collections()
        {
            User user = await _userRepository.GetUserByEmailAsync(User.Identity.Name);
            var collections = _collectionRepository.GetAllByUser(user);
            
            ViewData["User"] = user;
            return View(collections);
        }


        [HttpGet]
        public async Task<IActionResult> AddItem(int id)
        {
            var fields = await _customFieldRepository.GetAllAsync(id);
            User user = await _userRepository.GetUserByEmailAsync(User.Identity.Name);
            ViewData["User"] = user;
            ViewData["CollectionId"] = id;
            ViewData["CustomFields"] = fields;
            return View();
        }


        [HttpPost]
        public async Task<IActionResult> AddItem(int collectionId,  string name, string[] tags, int[] customField, string[] customFieldValue)
        {
            List<Tags> tags2 = new List<Tags>();
            foreach(string tag in tags)
            {
                tags2.Add(await _tagRepository.AddAsync(new Tags { Value = tag }));
            }

            List<CustomFieldValue> fields = new List<CustomFieldValue>();
            for (int i = 0; i < customFieldValue.Length; i++)
            {
                fields.Add(await _customFieldRepository.AddCustomFieldValueAsync(
                    new CustomFieldValue
                    {
                        Value = customFieldValue[i],
                        CustomField = customField[i],
                        CollectionId = collectionId
                    }));
            }

            await _itemRepository.AddAsync(collectionId, new Item { 
                Tags = tags2, 
                Name = name, 
                CollectionId = collectionId, 
                CustomFieldValues = fields });
            return RedirectToAction("Collection", "Collection", new { id = collectionId });
        }


        [HttpPost]
        public async Task<IActionResult> AddComment(int itemId, string text)
        {
            string name = User.Identity.Name;
            Item item = await _itemRepository.AddCommentAsync(itemId, text, name);
            return RedirectToAction("Item", new { id = item.Id});
        }


        [HttpGet]
        public async Task<IActionResult> AddLike(int id)
        {
            string name = User.Identity.Name;
            Item item = await _itemRepository.AddOrDeleteLikeAsync(id, name);
            User user = await _userRepository.GetUserByEmailAsync(User.Identity.Name);
            ViewData["User"] = user;
            return RedirectToAction("Item", new { id = item.Id });
        }


        [HttpGet]
        public async Task<IActionResult> EditCollection(int id)
        {
            Collection collection = await _collectionRepository.GetByIdAsync(id);
            User user = await _userRepository.GetUserByEmailAsync(User.Identity.Name);
            ViewData["User"] = user;
            return View("AddCollection", collection);
        }


        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Item(int id)
        {
            Item item = await _itemRepository.GetItemByIdAsync(id);
            StringBuilder builder = new StringBuilder();
            foreach(Tags tag in item.Tags)
            {
                builder.Append(tag.Value + ", ");
            }
            User user = await _userRepository.GetUserByEmailAsync(User.Identity.Name);
            ViewData["User"] = user;
            ViewData["Collection"] = await _collectionRepository.GetByIdAsync(item.CollectionId);
            ViewData["Field"] = await _customFieldRepository.GetAllAsync(item.CollectionId);
            ViewData["FieldValue"] = await _customFieldRepository.GetAllValuesAsync(item.CollectionId);
            ViewData["Tags"] = builder.ToString();
            return View(item);
        }


        [HttpGet]
        public async Task<IActionResult> DeleteCollection(int id)
        {
            await _customFieldRepository.DeleteValuesByIdAsync(id);
            await _customFieldRepository.DeleteByIdAsync(id);
            await _itemRepository.DeleteAsync(id);
            await _collectionRepository.DeleteAsync(id);

            return RedirectToAction("Collections");
        }


        [HttpGet]
        public async Task<IActionResult> AddCollection()
        {
            User user = await _userRepository.GetUserByEmailAsync(User.Identity.Name);
            ViewData["User"] = user;
            return View();
        }


        [HttpPost]
        public async Task<IActionResult> AddCollectionPost(IFormFile files, string description, string name, TypeItem theme, string[] field, CustomFieldType[] newFieldType)
        {
            int idCollection;
            if (files != null)
            {
                idCollection = await _collectionRepository.AddAsync(new Collection {
                    Name = name,
                    Description = description,
                    Type = theme,
                    PathImage = _googleRepository.UploadPhoto(files.OpenReadStream()),
                    User = await _userRepository.GetUserByEmailAsync(User.Identity.Name) 
                });
            } else
            {
                idCollection = await _collectionRepository.AddAsync(new Collection {
                    Name = name,
                    Description = description,
                    Type = theme,
                    User = await _userRepository.GetUserByEmailAsync(User.Identity.Name)
                });
            }

            for(int i = 0; i < field.Length; i++)
            {
                await _customFieldRepository.AddAsync(new CustomField
                {
                    CollectionId = idCollection,
                    Title = field[i],
                    CustomFieldType = newFieldType[i]
                });
            }

            return RedirectToAction("Collections", "Home");
        }



        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Search(string searchText)
        {
            User user = await _userRepository.GetUserByEmailAsync(User.Identity.Name);
            ViewData["User"] = user;
            return View();
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [HttpPost]
        [AllowAnonymous]
        public IActionResult SetLanguage(string culture, string returnUrl)
        {
            Response.Cookies.Append(
                CookieRequestCultureProvider.DefaultCookieName,
                CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture)),
                new CookieOptions { Expires = DateTimeOffset.UtcNow.AddYears(1) }
            );

            return LocalRedirect(returnUrl);
        }
    }
}