using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BeanBagIntegrationTests
{
    public class IntegrationBlobStorageTest
    {
        private static CloudStorageAccount account;
        private static CloudBlobClient blobClient;

        [ClassInitialize]
        public static void StartAndCleanStorage(TestContext cont)
        {
            account = CloudStorageAccount.DevelopmentStorageAccount;
            blobClient = account.CreateCloudBlobClient();
            //blobClient.StartEmulator();
        }

        [TestMethod]
        public void upload_item_image()
        {

        }
    }
}
