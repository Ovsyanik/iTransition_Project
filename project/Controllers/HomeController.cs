﻿using Microsoft.AspNetCore.Authorization;
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
            List<Collection> collection = _collectionRepository.GetAllByUser(
                await _userRepository.GetUserByEmail(User.Identity.Name));
            return View(collection);
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
                CustomFieldValue id = await _customFieldRepository.AddCustomFieldValueAsync(
                    new CustomFieldValue
                    {
                        Value = customFieldValue[i],
                        CustomField = customField[i],
                        CollectionId = collectionId
                    });
                fields.Add(id);
            }

            await _itemRepository.AddAsync(new Item { 
                Tags = tags2, 
                Name = name, 
                CollectionId = collectionId, 
                CustomFieldValues = fields });
            return RedirectToAction("Collection", new { id = collectionId });
        }


        [HttpGet]
        public IActionResult AddCollection()
        {
            return View();
        }


        [HttpPost]
        public async Task<IActionResult> AddCollectionPost(IFormFile files, string description, string name, TypeItem theme, string[] field, CustomFieldType[] newFieldType)
        {
            int idCollection = await _collectionRepository.AddAsync(new Collection(
                name, 
                description, 
                theme,
                _googleRepositoy.UploadPhoto(files.OpenReadStream()),
                await _userRepository.GetUserByEmail(User.Identity.Name)));

            for(int i = 0; i < field.Length; i++)
            {
                CustomField customField = new CustomField {
                    CollectionId = idCollection,
                    Title = field[i],
                    CustomFieldType = newFieldType[i]
                };

                await _customFieldRepository.AddAsync(customField);
            }

            return RedirectToAction("Collections", "Home");
        }


        [HttpGet]
        public async Task<IActionResult> Collection(int id)
        {
            List<Item> items = await _itemRepository.GetAllAsync(id);
            List<CustomField> customFields = await _customFieldRepository.GetAllAsync(id);
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


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
