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

namespace BeanBagIntegrationTests
{
    public class IntegrationBlobTestMu : IClassFixture<BlobStorageFixture>
    {

        [Fact]
        public async Task upload_Item_Image()
        {
            //ARRANGE

            var storageAccount = CloudStorageAccount.Parse("UseDevelopmentStorage=true");
            var blobClient = storageAccount.CreateCloudBlobClient();
            var container = blobClient.GetContainerReference("itemImages");

            var myserv = new BlobStorageService();
            

            //var storageaccount = cloudstorageaccount.parse(blobstorageconfiguration.local.connectionstring);
            //var blobclient = storageaccount.createcloudblobclient();
            //var container = blobclient.getcontainerreference("whitelist");

            var newImage = new Uri("http://" + Path.GetRandomFileName());
           

            


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

            var sut = new BlobStorageService();
            var file = fileMock.Object;


            //ACT


            await container.CreateIfNotExistsAsync();

            //await myserv.uploadItemImage(File);

            var result = await sut.uploadItemImage(file);


            //ASSERT
            using (var memoryStream = new MemoryStream())
            {
                await container.GetBlockBlobReference("test.pdf").DownloadToStreamAsync(memoryStream);
                var report = Encoding.UTF8.GetString(memoryStream.ToArray());
                Assert.Contains(file.ContentType, "application/pdf");

            }


        }

        [Fact]
            public async Task upload_Test_Images()
            {
            //ARRANGE
            IFormFileCollection _testImages;
            string projId = "";

            //ACT


            //ASSERT

            }

            [Fact]
            public async Task delete_Test_Image_Folder()
            {
            //ARRANGE
            string projId = "";

            //ACT


            //ASSERT

            }



        }


    public class BlobStorageFixture : IDisposable
    {
        readonly Process process;

        public BlobStorageFixture()
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

