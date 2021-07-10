using Microsoft.AspNetCore.Http;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace BeanBag.Services
{
    public class BlobStorageService : IBlobStorageService
    {
        private readonly CloudStorageAccount cloudStorageAccount;
        private readonly CloudBlobClient cloudBlobClient;
        private readonly CloudBlobContainer cloudBlobContainer;

        public BlobStorageService()
        {
            cloudStorageAccount = CloudStorageAccount.Parse("DefaultEndpointsProtocol=https;AccountName=polarisblobstorage;AccountKey=y3AJRr3uWZOtpxx3YxZ7MFIQY7oy6nQsYaEl6jFshREuPND4H6hkhOh9ElAh2bF4oSdmLdxOd3fr+ueLbiDdWw==;EndpointSuffix=core.windows.net");
            cloudBlobClient = cloudStorageAccount.CreateCloudBlobClient();
            cloudBlobContainer = cloudBlobClient.GetContainerReference("itemimages");
        }

        public async Task<string> uploadItemImage(IFormFile file)
        {
            CloudBlockBlob cloudBlockBlob = cloudBlobContainer.GetBlockBlobReference(file.FileName);
            cloudBlockBlob.Properties.ContentType = file.ContentType;

            // Copying the file into a memory stream and then uploaded into the azure blob container
            var ms = new MemoryStream();
            await file.CopyToAsync(ms);
            await cloudBlockBlob.UploadFromByteArrayAsync(ms.ToArray(), 0, (int)ms.Length);

            return cloudBlockBlob.Uri.AbsoluteUri.ToString();
        }


    }
}
