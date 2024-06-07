using System;
using System.IO;
using System.Threading.Tasks;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;

namespace project.Models.Repositories
{
    public class CloudRepository : ICloudRepository
    {
        private string apiName = "dz9um6woc";
        private string apiKey = "678355628328799";
        private string apiSecret = "Nc6G5_no7niQ-eTRZp_ptXezN6w";

        public string UploadPhoto(Stream stream)
        {
            Account account = new Account(apiName, apiKey, apiSecret);

            Cloudinary cloudinary = new Cloudinary(account);
            ImageUploadParams uploadParams = new ImageUploadParams()
            {
                File = new FileDescription(Guid.NewGuid().ToString(), stream),
            };

            ImageUploadResult uploadResult = cloudinary.Upload(uploadParams);

            Transformation transformation = new Transformation().Width(960).Height(600).Crop("fill");

            return cloudinary.Api.UrlImgUp.Transform(transformation)
                .BuildUrl($"{uploadResult.PublicId}.{uploadResult.Format}");
        }
    }
} 