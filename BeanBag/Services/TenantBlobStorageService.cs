using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;

namespace BeanBag.Services
{
    public class TenantBlobStorageService : ITenantBlobStorageService
    {
        private readonly CloudBlobContainer _tenantCloudBlobContainer;

        public TenantBlobStorageService()
        {
            var cloudStorageAccount = CloudStorageAccount.Parse("DefaultEndpointsProtocol=https;AccountName=polarisblobstorage;AccountKey=y3AJRr3uWZOtpxx3YxZ7MFIQY7oy6nQsYaEl6jFshREuPND4H6hkhOh9ElAh2bF4oSdmLdxOd3fr+ueLbiDdWw==;EndpointSuffix=core.windows.net");
            var cloudBlobClient = cloudStorageAccount.CreateCloudBlobClient();
            _tenantCloudBlobContainer = cloudBlobClient.GetContainerReference("tenant-logos");
        }
        
        
        public async Task<string> UploadLogoImage(IFormFile file)
        {
            var tenantCloudBlockBlob = _tenantCloudBlobContainer.GetBlockBlobReference(file.FileName);
            tenantCloudBlockBlob.Properties.ContentType = file.ContentType;

            var memoryStream = new MemoryStream();
            await file.CopyToAsync(memoryStream);
            await tenantCloudBlockBlob.UploadFromByteArrayAsync(memoryStream.ToArray(), 0, (int)memoryStream.Length);

            return tenantCloudBlockBlob.Uri.AbsoluteUri;
        }
    }
}