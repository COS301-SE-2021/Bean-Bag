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
        private readonly CloudBlobClient _cloudBlobClient;
        private CloudBlobContainer _cloudBlobContainer;
        //private readonly IConfiguration _config;

        // Constructor
        public BlobStorageService(IConfiguration config)
        {
            var cloudStorageAccount = CloudStorageAccount.Parse(config.GetValue<string>("AzureBlobStorage:ConnectionString"));

            //cloudStorageAccount = CloudStorageAccount.Parse("DefaultEndpointsProtocol=https;AccountName=polarisblobstorage;AccountKey=y3AJRr3uWZOtpxx3YxZ7MFIQY7oy6nQsYaEl6jFshREuPND4H6hkhOh9ElAh2bF4oSdmLdxOd3fr+ueLbiDdWw==;EndpointSuffix=core.windows.net");
            _cloudBlobClient = cloudStorageAccount.CreateCloudBlobClient();
        }
        
        /*
        public BlobStorageService()
        {
            throw new System.NotImplementedException();
        }*/
        

        // This method is used to upload an item image into the blob storage
        public async Task<string> UploadItemImage(IFormFile file)
        {
            _cloudBlobContainer = _cloudBlobClient.GetContainerReference("itemimages");
            CloudBlockBlob cloudBlockBlob = _cloudBlobContainer.GetBlockBlobReference(file.FileName);
            cloudBlockBlob.Properties.ContentType = file.ContentType;

            // Copying the file into a memory stream and then uploaded into the azure blob container
            var ms = new MemoryStream();
            await file.CopyToAsync(ms);
            await cloudBlockBlob.UploadFromByteArrayAsync(ms.ToArray(), 0, (int)ms.Length);

            return cloudBlockBlob.Uri.AbsoluteUri;
        }

        // This method is used to upload a set of test images used to train an AI model 
        public async Task<List<string>> UploadTestImages(IFormFileCollection testImages, string projectId)
        {
            List<string> testImagesUrls = new List<string>();


            _cloudBlobContainer = _cloudBlobClient.GetContainerReference("modeltestimages");

            foreach(var image in testImages)
            {
                var cloudBlockBlob = _cloudBlobContainer.GetBlockBlobReference(projectId + "/" + image.FileName);
                cloudBlockBlob.Properties.ContentType = image.ContentType;
                var ms = new MemoryStream();
                await image.CopyToAsync(ms);
                await cloudBlockBlob.UploadFromByteArrayAsync(ms.ToArray(), 0, (int)ms.Length);

                testImagesUrls.Add(cloudBlockBlob.Uri.AbsoluteUri);
            }

            return testImagesUrls;
            
        }

        // This method is used to delete a folder of test images used to train an AI model
        public async void DeleteTestImageFolder(string projectId)
        {
            _cloudBlobContainer = _cloudBlobClient.GetContainerReference("modeltestimages");
            
            await _cloudBlobContainer.GetBlockBlobReference(projectId).DeleteIfExistsAsync();
        }
    }
}
