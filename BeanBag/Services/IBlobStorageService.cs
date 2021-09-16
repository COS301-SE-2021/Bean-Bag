using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.FileProviders;

namespace BeanBag.Services
{
    // This class is an interface for the blob storage service.
    public interface IBlobStorageService
    {
        public Task<string> uploadItemImage(IFormFile file);

        public Task<List<string>> uploadTestImages(IDirectoryContents testImages, string projectId);

        public void deleteTestImageFolder(string projectId);
    }
}
