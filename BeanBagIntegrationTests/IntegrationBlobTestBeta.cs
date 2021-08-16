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

        public IntegrationBlobTestBeta(BlobStorageFixture2 fixture) { }


        public IntegrationBlobTestBeta()
        {
            cloudStorageAccount = CloudStorageAccount.Parse("UseDevelopmentStorage=true");
            cloudBlobClient = cloudStorageAccount.CreateCloudBlobClient();
        }

        [Fact]
        public async Task Upload_Item_Image()
        {
            //ARRANGE
            var fileMock = new Mock<IFormFile>();
            //Setup mock file using a memory stream
            var content = "Hello World from a Fake File";
            var fileName = "test.pdf";
            var ms = new MemoryStream();
            var writer = new StreamWriter(ms);
            writer.Write(content);
            writer.Flush();
            ms.Position = 0;
            fileMock.Setup(_ => _.OpenReadStream()).Returns(ms);
            fileMock.Setup(_ => _.FileName).Returns(fileName);
            fileMock.Setup(_ => _.Length).Returns(ms.Length);

            var file = fileMock.Object;

            cloudBlobContainer = cloudBlobClient.GetContainerReference("itemimagesBeta");

            CloudBlockBlob cloudBlockBlob = cloudBlobContainer.GetBlockBlobReference(file.FileName);
            cloudBlockBlob.Properties.ContentType = file.ContentType;

            var myser = new BlobStorageService();


            //ACT

            await myser.uploadItemImage(file);


            //ASSERT
        }



        [Fact]
        public async Task upload_Test_Images()
        {
        //ARRANGE
 

        //ACT


        //ASSERT

        }



        [Fact]
        public async Task delete_Test_Image_Folder()
        {
        //ARRANGE
      

        //ACT


        //ASSERT

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

