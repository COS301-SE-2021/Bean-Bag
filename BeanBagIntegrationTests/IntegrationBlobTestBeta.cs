using BeanBag;
using Microsoft.AspNetCore.Mvc.Testing;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;
using Xunit;
using Microsoft.AspNetCore.Http;
using BeanBag.Services;
using Microsoft.Extensions.Configuration;
using Moq;
using Microsoft.WindowsAzure.Storage.Blob;

namespace BeanBagIntegrationTests
{
    public class IntegrationBlobTestBeta : IClassFixture<BlobStorageFixture2>
    {
        private readonly CloudStorageAccount cloudStorageAccount;
        private readonly CloudBlobClient cloudBlobClient;
        private CloudBlobContainer cloudBlobContainer;

        public IntegrationBlobTestBeta()
        {
            cloudStorageAccount = CloudStorageAccount.Parse("DefaultEndpointsProtocol=https;AccountName=polarisblobstorage;AccountKey=y3AJRr3uWZOtpxx3YxZ7MFIQY7oy6nQsYaEl6jFshREuPND4H6hkhOh9ElAh2bF4oSdmLdxOd3fr+ueLbiDdWw==;EndpointSuffix=core.windows.net");
            cloudBlobClient = cloudStorageAccount.CreateCloudBlobClient();
        }

        [Fact]
        public async Task Upload_Item_Image()
        {
            //ARRANGE
            var fileMock = new Mock<IFormFile>();
            //Setup mock file using a memory stream
            var content = "File added for the purpose of integration testing!";
            
            var chars = "0123456789";
            var stringChars = new char[5];
            var random = new Random();

            for (int i = 0; i < stringChars.Length; i++)
            {
                stringChars[i] = chars[random.Next(chars.Length)];
            }

            var finalString = new String(stringChars);

            var myGuidEnd = finalString;

            string pdfEnd = finalString.Substring(0, 4);
            
            
            var fileName = "test" + pdfEnd + ".pdf";
            
            var ms = new MemoryStream();
            var writer = new StreamWriter(ms);
            writer.Write(content);
            writer.Flush();
            ms.Position = 0;
            fileMock.Setup(_ => _.OpenReadStream()).Returns(ms);
            fileMock.Setup(_ => _.FileName).Returns(fileName);
            fileMock.Setup(_ => _.Length).Returns(ms.Length);

            var file = fileMock.Object;

            var myService = new BlobStorageService();
            
            cloudBlobContainer = cloudBlobClient.GetContainerReference("itemimages");
            CloudBlockBlob cloudBlockBlob = cloudBlobContainer.GetBlockBlobReference(file.FileName);
            cloudBlockBlob.Properties.ContentType = file.ContentType;

            //ACT

            await myService.uploadItemImage(file);
            var myUploadedFile = cloudBlobContainer.GetBlockBlobReference(file.FileName);
            

            //ASSERT
            Assert.NotNull(myUploadedFile);
        }
        




        }


    public class BlobStorageFixture2 : IDisposable
    {
        readonly Process process;

        public BlobStorageFixture2()
        {
            process = new Process
            {
                StartInfo = {
                    UseShellExecute = false,
                    FileName = @"C:\Program Files (x86)\Microsoft SDKs\Azure\Storage Emulator\AzureStorageEmulator.exe",
                }
            };

            StartAndWaitForExit("stop");
            StartAndWaitForExit("clear all");
            StartAndWaitForExit("start");
        }

        public void Dispose()
        {
            StartAndWaitForExit("stop");
        }

        void StartAndWaitForExit(string arguments)
        {
            process.StartInfo.Arguments = arguments;
            process.Start();
            process.WaitForExit(10000);
        }
    }


}

