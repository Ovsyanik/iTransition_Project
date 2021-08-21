using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using project.Models;
using project.Models.Entities;
using project.Models.Repositories;

using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;

namespace project.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ItemRepository _itemRepository;
        private readonly TagRepository _tagRepository;
        private readonly CollectionRepository _collectionRepository;
        private readonly CustomFieldRepository _customFieldRepository;

        public HomeController(
            TagRepository tagRepository,
            ItemRepository itemReposetory, 
            CollectionRepository collectionRepository, 
            CustomFieldRepository customFieldRepository)
        {
            _tagRepository = tagRepository;
            _itemRepository = itemReposetory;
            _collectionRepository = collectionRepository;
            _customFieldRepository = customFieldRepository;
        }


        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Index()
        {
            ViewData["LastItems"] = await _itemRepository.GetLastItemsAsync();
            var collections = await _collectionRepository.GetCollectionsLargestItem();
            ViewData["CollectionLargestItems"] = collections;
            return View();
        }


        [HttpGet]
        public async Task<IActionResult> AddItem(int id, int itemId)
        {
            Item item = await _itemRepository.GetItemByIdAsync(itemId);
            var fields = await _customFieldRepository.GetAllAsync(id);
            ViewData["CollectionId"] = id;
            ViewData["CustomFields"] = fields;
            return View(item);
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
            return RedirectToAction("Item", new { id = item.Id });
        }


        public async Task<IActionResult> DeleteItem(int collectionId, int id)
        {
            await _itemRepository.DeleteAsync(collectionId, id);
            return RedirectToAction("Collections", "Collection");
        }


        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Item(int id)
        {
            Item item = await _itemRepository.GetItemByIdAsync(id);
            ViewData["Field"] = await _customFieldRepository.GetAllAsync(item.CollectionId);
            ViewData["FieldValue"] = await _customFieldRepository.GetAllValuesAsync(item.CollectionId);
            return View(item);
        }


        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> EditItem(int id)
        {
            Item item = await _itemRepository.GetItemByIdAsync(id);
            ViewData["Field"] = await _customFieldRepository.GetAllAsync(item.CollectionId);
            ViewData["FieldValue"] = await _customFieldRepository.GetAllValuesAsync(item.CollectionId);
            return View("AddItem", item);
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Search(string searchText)
        {
            List<Item> items = await _itemRepository.SearchItems(searchText);
            return View(items);
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


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}