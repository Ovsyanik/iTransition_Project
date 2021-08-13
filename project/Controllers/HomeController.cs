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
        private readonly CollectionRepository _collectionRepository;
        private readonly CustomFieldRepository _customFieldRepository;

        public HomeController(ItemRepository itemReposetory, UserRepository userRepository, 
            CloudRepository googleRepository, CollectionRepository collectionRepository, CustomFieldRepository customFieldRepository)
        {
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
        public IActionResult Collections()
        {
            List<Collection> collection = _collectionRepository.GetAllByUser(
                _userRepository.GetUserByEmail(User.Identity.Name));
            return View(collection);
        }


        [HttpGet]
        public IActionResult AddItem(int id)
        {
            ViewData["CollectionId"] = id;
            ViewData["CustomFields"] = _customFieldRepository.GetAll(id);
            return View();
        }


        [HttpPost]
        public IActionResult AddItem(string name, string tags, string[] customField)
        {
            return View();
        }


        [HttpGet]
        public IActionResult AddCollection()
        {
            return View();
        }


        [HttpPost]
        public IActionResult AddCollectionPost(IFormFile files, string description, string name, TypeItem theme, string[] field, CustomFieldType[] newFieldType)
        {
            int idCollection = _collectionRepository.Add(new Collection(
                name, 
                description, 
                theme,
                _googleRepositoy.UploadPhoto(files.OpenReadStream()),
                _userRepository.GetUserByEmail(User.Identity.Name)));

            for(int i = 0; i < field.Length; i++)
            {
                CustomField customField = new CustomField {
                    CollectionId = idCollection,
                    Title = field[i],
                    CustomFieldType = newFieldType[i]
                };

                _customFieldRepository.Add(customField);
            }

            return RedirectToAction("Collections", "Home");
        }


        [HttpGet]
        public IActionResult Collection(int id)
        {
            List<Item> items = _itemRepository.GetAll(id);
            List<CustomField> customFields = _customFieldRepository.GetAll(id);
            ViewData["CustomFields"] = customFields;
            ViewData["CollectionId"] = id;
            return View(items);
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Search(string searchText)
        {
            return View();
        }


        [HttpGet]
        public async Task<IActionResult> Admin()
        {
            List<User> users = await _userRepository.GetAllAsync();
            return View(users);
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
