using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace BeanBag.Services
{
    // This class is an interface for the Tenant Blob Storage service.
    public interface ITenantBlobStorageService
    {
        public Task<string> UploadLogoImage(IFormFile file);
    }
}