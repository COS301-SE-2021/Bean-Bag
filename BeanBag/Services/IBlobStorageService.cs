using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BeanBag.Services
{
    // This class is an interface for the blob storage service.
    public interface IBlobStorageService
    {
        public Task<string> UploadItemImage(IFormFile file);

        public Task<List<string>> UploadTestImages(IFormFileCollection testImages, string projectId);

        public void DeleteTestImageFolder(string projectId);
    }
}
