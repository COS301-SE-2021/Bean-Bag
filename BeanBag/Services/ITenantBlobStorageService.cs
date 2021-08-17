using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace BeanBag.Services
{
    public interface ITenantBlobStorageService
    {
        public Task<string> UploadLogoImage(IFormFile file);
    }
}