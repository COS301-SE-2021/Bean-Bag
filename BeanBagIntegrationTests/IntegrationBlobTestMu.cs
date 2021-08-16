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

namespace BeanBagIntegrationTests
{
    public class IntegrationBlobTestMu 
    {
        public class BlobStorageFixture : IDisposable {
            readonly Process process;

            public BlobStorageFixture() {
                process = new Process {
                    StartInfo = {
                        UseShellExecute = false,
                        FileName = @"C:\Program Files (x86)\Microsoft SDKs\Azure\Storage Emulator\AzureStorageEmulator.exe",
                    }
                };

                StartAndWaitForExit("stop");
                StartAndWaitForExit("clear all");
                StartAndWaitForExit("start");
            }

            public void Dispose() {
                StartAndWaitForExit("stop");
            }

            void StartAndWaitForExit(string arguments) {
                process.StartInfo.Arguments = arguments;
                process.Start();
                process.WaitForExit(10000);
            }
        

        public class FaceWhitelistTests : IClassFixture<BlobStorageFixture> {
            public FaceWhitelistTests(BlobStorageFixture fixture) { }

            [Fact]
            public async Task WhitelistUser_NewUser_IsWhitelisted() {
                var faceWhitelist = Create();
                var userToWhiteList = new SlackUser {Id = Path.GetRandomFileName()};

                await faceWhitelist.WhitelistUser(userToWhiteList);

                await VerifyUserIsWhiteListed(userToWhiteList);
            }

            // ... more test cases exist IRL

            static FaceWhitelist Create() =>
                new FaceWhitelist(BlobStorageConfiguration.Local);

            static async Task VerifyUserIsWhiteListed(SlackUser user) {
                var storageAccount = CloudStorageAccount.Parse(BlobStorageConfiguration.Local.ConnectionString);
                var blobClient = storageAccount.CreateCloudBlobClient();
                var container = blobClient.GetContainerReference("whitelist");
                using (var memoryStream = new MemoryStream()) {
                    await container.GetBlockBlobReference(user.Id).DownloadToStreamAsync(memoryStream);
                    var actualUserId = Encoding.UTF8.GetString(memoryStream.ToArray());
                    Assert.Equal(user.Id, actualUserId);
                }
            }
        }
        

    }
}
