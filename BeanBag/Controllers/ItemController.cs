using BeanBag.Database;
using BeanBag.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Azure.CognitiveServices.Vision.CustomVision.Prediction;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;


namespace BeanBag.Controllers
{
    public class ItemController : Controller
    {
        // This variable is used to interact with the Database/DBContext class. Allows us to save, update and delete records 
        private readonly DBContext _db;
        // The connection string is exposed. Will need to figure out a way of getting it out of appsetting.json. Maybe init in startup like db context ?
        
        public ItemController(DBContext db)
        {
            _db = db;
        }

        public IActionResult Index()
        {
            return Ok("Item Index");
        }

        
        public IActionResult UploadImage()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> UploadImage([FromForm(Name = "file")] IFormFile file)
        {

            CloudStorageAccount cloudStorageAccount = CloudStorageAccount.Parse("DefaultEndpointsProtocol=https;AccountName=polarisblobstorage;AccountKey=y3AJRr3uWZOtpxx3YxZ7MFIQY7oy6nQsYaEl6jFshREuPND4H6hkhOh9ElAh2bF4oSdmLdxOd3fr+ueLbiDdWw==;EndpointSuffix=core.windows.net");
            CloudBlobClient cloudBlobClient = cloudStorageAccount.CreateCloudBlobClient();
            CloudBlobContainer cloudBlobContainer = cloudBlobClient.GetContainerReference("itemimages");

            CloudBlockBlob cloudBlockBlob = cloudBlobContainer.GetBlockBlobReference(file.FileName);
            cloudBlockBlob.Properties.ContentType = file.ContentType;

            var ms = new MemoryStream();

            await file.CopyToAsync(ms);
            await cloudBlockBlob.UploadFromByteArrayAsync(ms.ToArray(), 0, (int)ms.Length);

            string predictionUrl = "https://uksouth.api.cognitive.microsoft.com/";


            CustomVisionPredictionClient predictionClient = new CustomVisionPredictionClient(new Microsoft.Azure.CognitiveServices.Vision.CustomVision.Prediction.ApiKeyServiceClientCredentials("f05b67634cc3441492a07f32553d996a"))
            {
                Endpoint = predictionUrl
            };

            var result = predictionClient.ClassifyImageUrl(new Guid("377f08bf-2813-43cd-aa41-b0e623b2beec"), "Iteration1", 
                new Microsoft.Azure.CognitiveServices.Vision.CustomVision.Prediction.Models.ImageUrl(cloudBlockBlob.Uri.AbsoluteUri.ToString()));

            return Ok(result.Predictions[0].TagName);
        }

        /*
                // This function will upload the image of the item into the polarisblobstorage itemsimage container
                [HttpPost]
                public async Task<IActionResult> UploadImage([FromForm(Name ="file")]IFormFile file)
                {

                    CloudStorageAccount cloudStorageAccount = CloudStorageAccount.Parse("DefaultEndpointsProtocol=https;AccountName=polarisblobstorage;AccountKey=y3AJRr3uWZOtpxx3YxZ7MFIQY7oy6nQsYaEl6jFshREuPND4H6hkhOh9ElAh2bF4oSdmLdxOd3fr+ueLbiDdWw==;EndpointSuffix=core.windows.net");
                    CloudBlobClient cloudBlobClient = cloudStorageAccount.CreateCloudBlobClient();
                    CloudBlobContainer cloudBlobContainer = cloudBlobClient.GetContainerReference("itemimages");

                    CloudBlockBlob cloudBlockBlob = cloudBlobContainer.GetBlockBlobReference(file.FileName);
                    cloudBlockBlob.Properties.ContentType = file.ContentType;

                    using(var ms = new MemoryStream())
                    {
                        await file.CopyToAsync(ms);
                        await cloudBlockBlob.UploadFromByteArrayAsync(ms.ToArray(), 0, (int)ms.Length);
                    }
                    return Redirect(cloudBlockBlob.Uri.AbsoluteUri);
                }
        */

        // Get method for create
        // Returns Create view
        public IActionResult Create()
        {
            IEnumerable<SelectListItem> InventoryDropDown = _db.Inventories.Select(i => new SelectListItem
            {
                Text = i.name,
                Value = i.Id.ToString()
            });

            ViewBag.InventoryDropDown = InventoryDropDown;

            return View();
        }

        // Post method for create
        // Takes in form from Create view to add a new item to the DB
        [HttpPost]
        public IActionResult Create(Item newItem)
        {
            //Grab URL link for image of item
            if(ModelState.IsValid)
            {
                _db.Items.Add(newItem);
                _db.SaveChanges();
                
                return LocalRedirect("/Inventory/ViewItems?InventoryId="+newItem.inventoryId.ToString());
            }
            // Redirect to the inventory item view page
            return View();
        }

        public IActionResult Edit(Guid? Id)
        {
            if(Id == null)
            {
                return NotFound();
            }

            var item = _db.Items.Find(Id);
            if(item == null)
            {
                return NotFound();
            }

            IEnumerable<SelectListItem> InventoryDropDown = _db.Inventories.Select(i => new SelectListItem
            {
                Text = i.name,
                Value = i.Id.ToString()
            });

            ViewBag.InventoryDropDown = InventoryDropDown;

            return View(item);
        }

        [HttpPost]
        public IActionResult EditPost(Item item)
        {
            if(ModelState.IsValid)
            {
                _db.Items.Update(item);
                _db.SaveChanges();

                return LocalRedirect("/Inventory/ViewItems?InventoryId=" + item.inventoryId.ToString());
            }

            return View();
        }

        public IActionResult Delete(Guid? Id)
        {
            if (Id == null)
            {
                return NotFound();
            }

            var item = _db.Items.Find(Id);
            if (item == null)
            {
                return NotFound();
            }

            ViewBag.InventoryName = _db.Inventories.Find(item.inventoryId).name;
            return View(item);
        }

        [HttpPost]
        public IActionResult DeletePost(Guid? Id)
        {
            var item = _db.Items.Find(Id);

            if(item == null)
            {
                return NotFound();
            }

            _db.Items.Remove(item);
            _db.SaveChanges();
            return LocalRedirect("/Inventory/ViewItems?InventoryId=" + item.inventoryId.ToString());
        }
    }
}
