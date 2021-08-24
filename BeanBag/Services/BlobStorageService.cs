using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace BeanBag.Services
{
    // This service class is used to upload images related to the Bean Bag application into the Azure blob storage
    public class BlobStorageService : IBlobStorageService
    {
        // Variables 
        private readonly CloudStorageAccount cloudStorageAccount;
        private readonly CloudBlobClient cloudBlobClient;
        private CloudBlobContainer cloudBlobContainer;
        private readonly IConfiguration _config;

        // Constructor
        public BlobStorageService(IConfiguration config)
        {
            cloudStorageAccount = CloudStorageAccount.Parse(config.GetValue<string>("AzureStorage:ConnectionString"));

            //cloudStorageAccount = CloudStorageAccount.Parse("DefaultEndpointsProtocol=https;AccountName=polarisblobstorage;AccountKey=y3AJRr3uWZOtpxx3YxZ7MFIQY7oy6nQsYaEl6jFshREuPND4H6hkhOh9ElAh2bF4oSdmLdxOd3fr+ueLbiDdWw==;EndpointSuffix=core.windows.net");
            cloudBlobClient = cloudStorageAccount.CreateCloudBlobClient();
        }
         

        // This method is used to upload an item image into the blob storage
        public async Task<string> uploadItemImage(IFormFile file)
        {
            cloudBlobContainer = cloudBlobClient.GetContainerReference("itemimages");
            CloudBlockBlob cloudBlockBlob = cloudBlobContainer.GetBlockBlobReference(file.FileName);
            cloudBlockBlob.Properties.ContentType = file.ContentType;

            // Copying the file into a memory stream and then uploaded into the azure blob container
            var ms = new MemoryStream();
            await file.CopyToAsync(ms);
            await cloudBlockBlob.UploadFromByteArrayAsync(ms.ToArray(), 0, (int)ms.Length);

            return cloudBlockBlob.Uri.AbsoluteUri.ToString();
        }

        // This method is used to upload a set of test images used to train an AI model 
        public async Task<List<string>> uploadTestImages(IFormFileCollection testImages, string projectId)
        {
            List<string> testImagesUrls = new List<string>();
            CloudBlockBlob cloudBlockBlob;
            

            cloudBlobContainer = cloudBlobClient.GetContainerReference("modeltestimages");

            foreach(var image in testImages)
            {
                cloudBlockBlob = cloudBlobContainer.GetBlockBlobReference(projectId + "/" + image.FileName);
                cloudBlockBlob.Properties.ContentType = image.ContentType;
                var ms = new MemoryStream();
                await image.CopyToAsync(ms);
                await cloudBlockBlob.UploadFromByteArrayAsync(ms.ToArray(), 0, (int)ms.Length);

                testImagesUrls.Add(cloudBlockBlob.Uri.AbsoluteUri.ToString());
            }

            return testImagesUrls;
            
        }

        // This method ius used to delete a folder of test images used to train an AI model
        public async void deleteTestImageFolder(string projectId)
        {
            cloudBlobContainer = cloudBlobClient.GetContainerReference("modeltestimages");
            
            await cloudBlobContainer.GetBlockBlobReference(projectId).DeleteIfExistsAsync();
        }
    }
}
