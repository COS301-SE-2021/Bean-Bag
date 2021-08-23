using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BeanBag.Services
{
    public interface IBlobStorageService
    {
        public Task<string> uploadItemImage(IFormFile file);

        public Task<List<string>> uploadTestImages(IFormFileCollection testImages, string projectId);

        public void deleteTestImageFolder(string projectId);
    }
}
