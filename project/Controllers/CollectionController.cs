using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using project.Models.Entities;
using project.Models.Repositories;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace project.Controllers
{

    [Authorize(Roles = "Admin, User")]
    public class CollectionController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public CollectionController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [AllowAnonymous]
        public async Task<IActionResult> Collections(string email)
        {
            IdentityUser user = await _unitOfWork.Users.GetUserByEmailAsync(email ?? User.Identity.Name);
            List<Collection> collections = await _unitOfWork.Collections.GetAllByUserAsync(user);
            ViewData["UserEmail"] = email;
            return View("Collections", collections);
        }


        [AllowAnonymous]
        public async Task<IActionResult> Collection(int id)
        {
            Collection collection = await _unitOfWork.Collections.GetByIdAsync(id);
            return View(collection);
        }


        [HttpPost]
        public async Task<IActionResult> Filter(int collectionId, string field, string text)
        {
            Collection collection = null;

            if(field.Equals("Name"))
            {
                collection = await _unitOfWork.Collections.FilterByName(collectionId, text);
            } 
            else if(field.Equals("Tags"))
            {
                collection = await _unitOfWork.Collections.FilterByTags(collectionId, text);
            }

            return View("Collection", collection);
        }


        [HttpGet]
        public IActionResult AddCollection(string userEmail)
        {
            ViewData["UserEmail"] = userEmail;
            return View(userEmail);
        }


        public async Task<IActionResult> EditCollection(string userEmail, int id)
        {
            Collection collection = await _unitOfWork.Collections.GetByIdAsync(id);
            ViewData["UserEmail"] = userEmail;
            return View("AddCollection", collection);
        }


        [HttpPost]
        public async Task<IActionResult> AddCollectionPost(string userEmail, IFormFile files, Collection newCollection, string[] field, CustomFieldType[] newFieldType)
        {
            int idCollection = await _unitOfWork.Collections.AddAsync(new Collection
            {
                Name = newCollection.Name,
                Description = newCollection.Description,
                Type = newCollection.Type,
                PathImage = files != null ? _unitOfWork.Cloud.UploadPhoto(files.OpenReadStream()) : null,
                User = await _unitOfWork.Users.GetUserByEmailAsync(userEmail ?? User.Identity.Name)
            });

            for (int i = 0; i < field.Length; i++)
            {
                await _unitOfWork.CustomFields.AddAsync(new CustomField
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
            await _unitOfWork.Collections.DeleteAsync(id);
            return RedirectToAction("Collections", new { email = userEmail });
        }

        [HttpPost]
        public async Task<IActionResult> EditCollection(Collection collection, IFormFile files, int[] fieldId, string[] field, CustomFieldType[] newFieldType)
        {
            if(files != null)
            {
                collection.PathImage = _unitOfWork.Cloud.UploadPhoto(files.OpenReadStream());
            }

            for (int i = 0; i < field.Length; i++)
            {
                if (fieldId != null )
                {
                    if (fieldId.Length != 0)
                    {
                        await _unitOfWork.CustomFields.EditCustomFieldAsync(fieldId[i], field[i], newFieldType[i]);
                        fieldId = fieldId.Where((source, index) => index != i).ToArray();
                        field = field.Where((source, index) => index != i).ToArray();
                        newFieldType = newFieldType.Where((source, index) => index != i).ToArray();
                        i--;
                    }
                    else
                    {
                        await _unitOfWork.CustomFields.AddAsync(new CustomField
                        {
                            Title = field[i],
                            CollectionId = collection.Id,
                            CustomFieldType = newFieldType[i]
                        });
                    }
                } 
            }
            collection = await _unitOfWork.Collections.EditCollection(collection);
            return RedirectToAction("Collections", new { email = collection.User.Email});
        }

        [AllowAnonymous]
        public async Task<IActionResult> SortById(int id) {
            Collection collection = await _unitOfWork.Collections.SortByIdAsync(id);
            return View("Collection", collection);
        }


        [AllowAnonymous]
        public async Task<IActionResult> SortByName(int id)
        {
            Collection collection = await _unitOfWork.Collections.SortByNameAsync(id);
            return View("Collection", collection);
        }
    }
}