using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
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
        private readonly BlobServiceClient blobServiceClient;

        // Constructor
        public BlobStorageService(IConfiguration config)
        {
            cloudStorageAccount = CloudStorageAccount.Parse(config.GetValue<string>("AzureBlobStorage:ConnectionString"));

            cloudBlobClient = cloudStorageAccount.CreateCloudBlobClient();

            blobServiceClient = new BlobServiceClient(config.GetValue<string>("AzureBlobStorage:ConnectionString"));
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
        public async Task<List<string>> uploadModelImages(IFormFileCollection testImages, string projectId)
        {
            List<string> testImagesUrls = new List<string>();
            CloudBlockBlob cloudBlockBlob;


            cloudBlobContainer = cloudBlobClient.GetContainerReference("modeltestimages");

            foreach (var image in testImages)
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

        public void uploadModelImageToTempFolder(IFormFile file, string userId)
        {
            cloudBlobContainer = cloudBlobClient.GetContainerReference("modeltestimages");
            cloudBlobContainer.GetBlockBlobReference(userId);
            CloudBlockBlob cloudBlockBlob = cloudBlobContainer.GetBlockBlobReference(userId + "/" + file.FileName);
            cloudBlockBlob.Properties.ContentType = file.ContentType;

            var ms = new MemoryStream();
            file.CopyTo(ms);
            cloudBlockBlob.UploadFromByteArrayAsync(ms.ToArray(), 0, (int)ms.Length);
        }

        public async void deleteModelImageTempFolder(string userId)
        {
            var blobContainerClient = blobServiceClient.GetBlobContainerClient("modeltestimages");
            var blobItems = blobContainerClient.GetBlobsAsync(prefix: userId);

            await foreach(BlobItem blobItem in blobItems)
            {
                await blobContainerClient.GetBlobClient(blobItem.Name).DeleteIfExistsAsync();
            }

        }

    }
}
