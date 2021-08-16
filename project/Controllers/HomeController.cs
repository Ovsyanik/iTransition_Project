using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using project.Models;
using project.Models.Entities;
using project.Models.Repositories;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace project.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ItemRepository _itemRepository;
        private readonly UserRepository _userRepository;
        private readonly CloudRepository _googleRepositoy;
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
            _googleRepositoy = googleRepository;
            _collectionRepository = collectionRepository;
            _customFieldRepository = customFieldRepository;
        }


        [HttpGet]
        [AllowAnonymous]
        public IActionResult Index()
        {
            return View(_itemRepository.GetCollections());
        }


        [HttpGet]
        public async Task<IActionResult> Collections()
        {
            List<Collection> collections = _collectionRepository.GetAllByUser(
                await _userRepository.GetUserByEmail(User.Identity.Name));
            return View(collections);
        }


        [HttpGet]
        public async Task<IActionResult> AddItem(int id)
        {
            ViewData["CollectionId"] = id;
            ViewData["CustomFields"] = await _customFieldRepository.GetAllAsync(id);
            return View();
        }


        [HttpPost]
        public async Task<IActionResult> AddItem(int collectionId,  string name, string tags, int[] customField, string[] customFieldValue)
        {
            string[] tags1 = tags.Replace(" ", "").Split(",");
            List<Tags> tags2 = new List<Tags>();
            foreach(string s in tags1)
            {
                Tags tag = _tagRepository.Add(new Tags { Value = s });
                tags2.Add(tag);
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

            await _itemRepository.AddAsync(new Item { 
                Tags = tags2, 
                Name = name, 
                CollectionId = collectionId, 
                CustomFieldValues = fields });
            return RedirectToAction("Collection", new { id = collectionId });
        }


        [HttpGet]
        public async Task<IActionResult> EditCollection(int id)
        {
            Collection collection = await _collectionRepository.GetByIdAsync(id);
            return View("AddCollection", collection);
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
        public IActionResult AddCollection()
        {
            return View();
        }


        [HttpPost]
        public async Task<IActionResult> AddCollectionPost(IFormFile files, string description, string name, TypeItem theme, string[] field, CustomFieldType[] newFieldType)
        {
            int idCollection;
            if (files != null)
            {
                idCollection = await _collectionRepository.AddAsync(new Collection(
                    name,
                    description,
                    theme,
                    _googleRepositoy.UploadPhoto(files.OpenReadStream()),
                    await _userRepository.GetUserByEmail(User.Identity.Name)));
            } else
            {
                idCollection = await _collectionRepository.AddAsync(new Collection(
                    name,
                    description,
                    theme,
                    await _userRepository.GetUserByEmail(User.Identity.Name)));
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
        public async Task<IActionResult> Collection(int id)
        {
            ViewData["CustomFields"] = await _customFieldRepository.GetAllAsync(id);
            ViewData["CollectionId"] = id;
            return View(await _itemRepository.GetAllAsync(id));
        }


        [HttpGet]
        [AllowAnonymous]
        public IActionResult Search(string searchText)
        {
            return View();
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
