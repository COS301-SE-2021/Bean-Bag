using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;

namespace BeanBag.Services
{
    public class TenantBlobStorageService : ITenantBlobStorageService
    {
        private readonly CloudBlobContainer _tenantCloudBlobContainer;

        public TenantBlobStorageService(IConfiguration config)
        {
            var cloudStorageAccount = CloudStorageAccount.Parse(config.GetValue<string>("AzureBlobStorage:ConnectionString"));
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