using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Util.Store;
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

namespace project.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ItemRepository _bookRepository;
        private readonly UserRepository _userRepository;

        public HomeController(ItemRepository bookReposetory, UserRepository userRepository)
        {
            _bookRepository = bookReposetory;
            _userRepository = userRepository;
        }


        [HttpGet]
        [AllowAnonymous]
        public IActionResult Index()
        {
            return View(_bookRepository.GetCollections());
        }


        [HttpGet]
        public IActionResult Collections()
        {
            return View();
        }


        [HttpGet]
        public IActionResult AddCollection()
        {
            return View();
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

        public static Google.Apis.Drive.v3.DriveService GetService()
        {
            UserCredential credential;
            var CSPath = System.Web.Hosting.HostingEnvironment.MapPath("~/Content/");

            using (var stream = new FileStream(Path.Combine(CSPath, "client_secret.json"), FileMode.Open, FileAccess.Read))
            {
                string filePath = Path.Combine(CSPath, "DriveServiceCredentials.json");

                credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                    GoogleClientSecrets.Load(stream).Secrets,
                    Scopes,
                    "user",
                    System.Threading.CancellationToken.None,
                    new FileDataStore(filePath, true)).Result;
            }

            Google.Apis.Drive.v3.DriveService service = new Google.Apis.Drive.v3.DriveService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = "GoogleDriveRestAPI-v3"
            });

            return service;
        }

        public static void CreateFolderOnDrive(string Folder_Name)
        {
            Google.Apis.Drive.v3.DriveService service = GetService();

            Google.Apis.Drive.v3.Data.File FileMetaData = new
            Google.Apis.Drive.v3.Data.File();
            FileMetaData.Name = Folder_Name;
            FileMetaData.MimeType = "application/vnd.google-apps.folder";

            Google.Apis.Drive.v3.FilesResource.CreateRequest request;

            request = service.Files.Create(FileMetaData);
            request.Fields = "id";
            var file = request.Execute();
        }

        public static void FileUploadInFolder(string folderId, HttpPostedFileBase file)
        {
            if (file != null && file.ContentLength > 0)
            {
                Google.Apis.Drive.v3.DriveService service = GetService();

                string path = Path.Combine(HttpContext.Current.Server.MapPath("~/GoogleDriveFiles"),
                Path.GetFileName(file.FileName));
                file.SaveAs(path);

                var FileMetaData = new Google.Apis.Drive.v3.Data.File()
                {
                    Name = Path.GetFileName(file.FileName),
                    MimeType = MimeMapping.GetMimeMapping(path),
                    Parents = new List<string>
                    {
                        folderId
                    }
                };
                Google.Apis.Drive.v3.FilesResource.CreateMediaUpload request;
                using (var stream = new System.IO.FileStream(path,
                System.IO.FileMode.Open))
                {
                    request = service.Files.Create(FileMetaData, stream,
                    FileMetaData.MimeType);
                    request.Fields = "id";
                    request.Upload();
                }
                var file1 = request.ResponseBody;
            }
        }
    }
}
