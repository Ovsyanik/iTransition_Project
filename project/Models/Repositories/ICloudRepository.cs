using System.IO;

namespace project.Models.Repositories
{
    public interface ICloudRepository
    {
        string UploadPhoto(Stream stream);
    }
}