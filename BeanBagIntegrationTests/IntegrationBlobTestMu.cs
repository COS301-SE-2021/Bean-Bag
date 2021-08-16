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

namespace BeanBagIntegrationTests
{
    public class IntegrationBlobTestMu
    {
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


            public void upload_Item_Image(IFormFile file)
            {
                //ARRANGE


                //ACT


                //ASSERT

                
            }

            public void upload_Test_Images(IFormFileCollection testImages, string projectId)
            {
                //ARRANGE


                //ACT


                //ASSERT

            }

            public void delete_Test_Image_Folder(string projectId)
            {
                //ARRANGE


                //ACT


                //ASSERT

            }



        }
    }
}
