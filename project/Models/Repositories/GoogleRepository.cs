using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Threading;
using Google.Apis.Download;
using Google.Apis.Drive.v3;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using Microsoft.AspNetCore.Hosting;
using Google.Apis.Auth.OAuth2;
using System.Threading.Tasks;

namespace project.Models.Repositories
{
    public class GoogleRepository
    {
        private static string[] Scopes = { DriveService.Scope.Drive };
        private static string ApplicationName = "GoogleDriveAPIStart";
        private static string FolderId = "1OlEo-PebvbMh0eWqWswVmY6Ufn7Qm3ah";

        private static UserCredential GetUserCredential() {
            using (var stream = new FileStream("credentials.json", FileMode.Open, FileAccess.Read))
            {
                string creadPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
                creadPath = Path.Combine(creadPath, "driverApiCredentials", "drive-credentials.json");

                return GoogleWebAuthorizationBroker.AuthorizeAsync(
                    GoogleClientSecrets.Load(stream).Secrets,
                    Scopes,
                    "User",
                    CancellationToken.None,
                    new FileDataStore(creadPath, true)).Result;
            }     
        }
        
        private static DriveService GetDriveService(UserCredential credential)
        {
            return new DriveService(
                new BaseClientService.Initializer
                {
                    HttpClientInitializer = credential,
                    ApplicationName = ApplicationName
                });
        }

        private static async Task<string> UploadFileToDrive(DriveService service, string fileName, string filePath, string contentType)
        {
            var fileMetadata = new Google.Apis.Drive.v3.Data.File();
            fileMetadata.Name = fileName;
            fileMetadata.Parents = new List<string> { FolderId };

            FilesResource.CreateMediaUpload request;

            using (var stream = new FileStream(filePath, FileMode.Open))
            {
                request = service.Files.Create(fileMetadata, stream, contentType);
                await request.UploadAsync();
            }

            var file = request.ResponseBody;

            return file.Id;
        }
    }
} 