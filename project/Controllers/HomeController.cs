using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Diagnostics;
using project.Models;
using project.Models.Entities;
using project.Models.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;

namespace project.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public HomeController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }


        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Index()
        {
            ViewData["LastItems"] = await _unitOfWork.Items.GetLastItemsAsync();
            ViewData["CollectionLargestItems"] = await _unitOfWork.Collections.GetCollectionsLargestItemAsync();
            ViewData["TagsList"] = await _unitOfWork.Tags.GetAllValuesAsync();
            return View();
        }


        [HttpGet]
        public async Task<IActionResult> AddItem(int id, int itemId)
        {
            Item item = await _unitOfWork.Items.GetByIdAsync(itemId);
            ViewData["CollectionId"] = id;
            ViewData["CustomFields"] = await _unitOfWork.CustomFields.GetAllAsync(id);
            ViewData["tags"] = await _unitOfWork.Tags.GetAllAsync();
            return View(item);
        }


        [HttpPost]
        public async Task<IActionResult> AddItem(int collectionId,  string name, string[] tags, int[] customField, string[] customFieldValue)
        {
            List<Tags> tags2 = new List<Tags>();
            foreach(string tag in tags)
            {
                tags2.Add(await _unitOfWork.Tags.AddAsync(new Tags { Value = tag }));
            }

            List<CustomFieldValue> fields = new List<CustomFieldValue>();
            for (int i = 0; i < customFieldValue.Length; i++)
            {
                fields.Add(await _unitOfWork.CustomFields.AddCustomFieldValueAsync(
                    new CustomFieldValue
                    {
                        Value = customFieldValue[i],
                        CustomField = customField[i],
                        CollectionId = collectionId
                    }));
            }

            await _unitOfWork.Items.AddAsync(collectionId, new Item { 
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
            Item item = await _unitOfWork.Items.AddCommentAsync(itemId, text, name);
            return RedirectToAction("Item", new { id = item.Id});
        }


        [HttpGet]
        public async Task<IActionResult> AddLike(int id)
        {
            string name = User.Identity.Name;
            Item item = await _unitOfWork.Items.AddOrDeleteLikeAsync(id, name);
            return RedirectToAction("Item", new { id = item.Id });
        }


        public async Task<IActionResult> DeleteItem(int collectionId, int id)
        {
            await _unitOfWork.Items.DeleteAsync(collectionId, id);
            return RedirectToAction("Collections", "Collection");
        }


        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Item(int id)
        {
            Item item = await _unitOfWork.Items.GetByIdAsync(id);
            ViewData["Field"] = await _unitOfWork.CustomFields.GetAllAsync(item.CollectionId);
            ViewData["FieldValue"] = await _unitOfWork.CustomFields.GetAllValuesAsync(item.CollectionId);
            return View(item);
        }


        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> GetComments(int itemId)
        {
            List<Comment> comments = await _unitOfWork.Items.GetCommentsAsync(itemId);
            return Json(comments);
        }


        [HttpGet]
        public async Task<IActionResult> EditItem(int id)
        {
            Item item = await _unitOfWork.Items.GetByIdAsync(id);
            ViewData["Field"] = await _unitOfWork.CustomFields.GetAllAsync(item.CollectionId);
            ViewData["FieldValue"] = await _unitOfWork.CustomFields.GetAllValuesAsync(item.CollectionId);
            ViewData["CollectionId"] = item.Collection.Id;
            ViewData["tags"] = await _unitOfWork.Tags.GetAllAsync();
            ViewData["CustomFields"] = await _unitOfWork.CustomFields.GetAllAsync(item.Collection.Id);
            return View("AddItem", item);
        }


        [HttpPost]
        public async Task<IActionResult> EditItem(Item item, string[] tags, int[] fieldId, string[] customFieldValue)
        {
            await _unitOfWork.Items.EditAsync(item, tags, fieldId, customFieldValue);
            return RedirectToAction("Item", new { id = item.Id });
        }

        [HttpPost]
        public async Task<IActionResult> DeleteTag(int id, int idTag)
        {
            await _unitOfWork.Items.DeleteTagAsync(id, idTag);
            return Ok();
        }
        

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Search(string searchText)
        {
            List<Item> items = await _unitOfWork.Items.SearchItems(searchText);
            return View(items);
        }


        [HttpPost]
        public async Task<IActionResult> GetTags(string text)
        {
            List<Tags> tags = await _unitOfWork.Tags.GetAllAsync();
            return Json(tags);
        }


        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetItemByTag(string tag)
        {
            List<Item> items = await _unitOfWork.Items.GetByTagAsync(tag);
            return View("Search", items);
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


        [HttpPost]
        [AllowAnonymous]
        public IActionResult SetTheme(string theme, string returnUrl)
        {
            Response.Cookies.Append(
                "Theme",
                theme,
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