using BeanBag.Database;
using BeanBag.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Azure.CognitiveServices.Vision.CustomVision.Prediction;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Identity.Web;
using System.Drawing.Imaging;
using QRCoder;


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

        // Might need to implement
        // Currently not being used
        public IActionResult Index()
        {
            return Ok("Item Index");
        }

        // This returns the uploadimage view for item
        public IActionResult UploadImage(Guid inventoryId)
        {
            return View();
        }

        // This takes in an image file to be uploaded into the Azure blob container
        // This method also uses the custom vision AI that will predict what type of item is in the image
        [HttpPost]
        public async Task<IActionResult> UploadImage([FromForm(Name = "file")] IFormFile file)
        {
            // Objects used to upload the file into the Azure blob container
            CloudStorageAccount cloudStorageAccount = CloudStorageAccount.Parse("DefaultEndpointsProtocol=https;AccountName=polarisblobstorage;AccountKey=y3AJRr3uWZOtpxx3YxZ7MFIQY7oy6nQsYaEl6jFshREuPND4H6hkhOh9ElAh2bF4oSdmLdxOd3fr+ueLbiDdWw==;EndpointSuffix=core.windows.net");
            CloudBlobClient cloudBlobClient = cloudStorageAccount.CreateCloudBlobClient();
            CloudBlobContainer cloudBlobContainer = cloudBlobClient.GetContainerReference("itemimages");

            CloudBlockBlob cloudBlockBlob = cloudBlobContainer.GetBlockBlobReference(file.FileName);
            cloudBlockBlob.Properties.ContentType = file.ContentType;

            // Copying the file into a memory stream and then uploaded into the azure blob container
            var ms = new MemoryStream();
            await file.CopyToAsync(ms);
            await cloudBlockBlob.UploadFromByteArrayAsync(ms.ToArray(), 0, (int)ms.Length);

            // Objects used to predict what type of item is in the image
            string predictionUrl = "https://uksouth.api.cognitive.microsoft.com/";

            CustomVisionPredictionClient predictionClient = new CustomVisionPredictionClient(new Microsoft.Azure.CognitiveServices.Vision.CustomVision.Prediction.ApiKeyServiceClientCredentials("f05b67634cc3441492a07f32553d996a"))
            {
                Endpoint = predictionUrl
            };

            // We are using the URL: cloudBlockBlob.Uri.AbsoluteUri
            // This url is from the method above where we uploaded the image to the blob container
            // This method uses an image url and information regarding the AI model used to return the predicted item type
            var result = predictionClient.ClassifyImageUrl(new Guid("377f08bf-2813-43cd-aa41-b0e623b2beec"), "Iteration1", 
                new Microsoft.Azure.CognitiveServices.Vision.CustomVision.Prediction.Models.ImageUrl(cloudBlockBlob.Uri.AbsoluteUri.ToString()));

            // We are returning to the item create page with the imageURL and itemtype given into the parameters
            // These url parameters values are grabbed and put into the create item view fields. This makes the automation part of creating an item.
            return LocalRedirect("/Item/Create?imageUrl="+ cloudBlockBlob.Uri.AbsoluteUri.ToString() + "&itemType="+ result.Predictions[0].TagName);
        }

        // Get method for create
        // Returns Create view
        // The create page needs to accept an imageURL and item type
        // This method is only called once the image of the item is uploaded
        public IActionResult Create(string imageUrl, string itemType)
        {
            // This creates a list of the different inventories available to put the item into

            var inventories = from i in _db.Inventories where i.userId.Equals(User.GetObjectId()) select i;

            IEnumerable < SelectListItem > InventoryDropDown = inventories.Select(i => new SelectListItem
            {
                Text = i.name,
                Value = i.Id.ToString()
            });

            // View bag allows the controller to pass information into the view
            // We are passing the inventoryDrop drown as well as the automated imageURL and itemType from the imageUpload POST method

            ViewBag.InventoryDropDown = InventoryDropDown;
            ViewBag.itemType = itemType;
            ViewBag.imageUrl = imageUrl;

            return View();
        }

        // Post method for create
        // Takes in form from Create view to add a new item to the DB
        [HttpPost]
        public IActionResult Create(Item newItem)
        {
            // Adding to QRContents for QR Code scanning

            // Making sure the newItem is valid before adding it into the item table (making sure all the required fields have a value)
            if(ModelState.IsValid)
            {
                _db.Items.Add(newItem);
                _db.SaveChanges();

                string itemID = newItem.Id.ToString();

                // Adding QRContents to the new item made and applying changes
                newItem.QRContents = "https://bean-bag.azurewebsites.net/api/QRCodeScan?itemID=" + itemID;
                _db.Items.Update(newItem);
                _db.SaveChanges();
                
                // Returns back to the viewItems view for the inventory using the inventoryId
                return LocalRedirect("/Inventory/ViewItems?InventoryId="+newItem.inventoryId.ToString());
            }
            // Redirect to create view if the item model is invalid
            return View(newItem);
        }

        // This is the GET method of item edit
        // This returns the view of an item that is being edited
        // Accepts an itemId in order to return a view of the item that needs to be edited with it's respective information passed along
        public IActionResult Edit(Guid? Id)
        {
            if(Id == null)
            {
                return NotFound();
            }

            var item = _db.Items.Find(Id);
            // Does the item exist in the table 
            if(item == null)
            {
                return NotFound();
            }

            // This creates a list of the different inventories available to put the item into
            var inventories = from i in _db.Inventories where i.userId.Equals(User.GetObjectId()) select i;

            IEnumerable<SelectListItem> InventoryDropDown = inventories.Select(i => new SelectListItem
            {
                Text = i.name,
                Value = i.Id.ToString()
            });

            // View bag allows the controller to pass information into the view
            ViewBag.InventoryDropDown = InventoryDropDown;
            ViewBag.InventoryId = item.inventoryId;

            return View(item);
        }

        // This is the POST method for edit Item
        [HttpPost]
        public IActionResult EditPost(Item item)
        {
            // Makes sure that 
            if(ModelState.IsValid)
            {
                _db.Items.Update(item);
                _db.SaveChanges();

                return LocalRedirect("/Inventory/ViewItems?InventoryId=" + item.inventoryId.ToString());
            }

            return View();
        }

        // This is the GET method for delete item
        public IActionResult Delete(Guid? Id)
        {
 
            if (Id == null)
            {
                return NotFound();
            }

            var item = _db.Items.Find(Id);
            // Does the item exist in the item table
            if (item == null)
            {
                return NotFound();
            }

            ViewBag.InventoryName = _db.Inventories.Find(item.inventoryId).name;
            return View(item);
        }

        // This is the POST method for delete item
        [HttpPost]
        public IActionResult DeletePost(Guid? Id)
        {
            // Item variable
            var item = _db.Items.Find(Id);

            if(item == null)
            {
                return NotFound();
            }

            // Deletes item in table
            _db.Items.Remove(item);
            _db.SaveChanges();

            // Returns back to the view items in inventory using the inventory ID
            // The reason why we can still use item.inventoryId is because it is still an intact variable
            // The item variable is not deleted. Only the information in the item table, not the item variable itself
            return LocalRedirect("/Inventory/ViewItems?InventoryId=" + item.inventoryId.ToString());
        }

        // Define a function to generate a QR code every time we want to view it
        public IActionResult ViewQRCode(Guid Id)
        {
            var item = _db.Items.Find(Id);

            if(item == null)
            {
                return NotFound();
            }
            else
            {
                var ms = new MemoryStream();
                var qRCodeGenerator = new QRCodeGenerator();
                var qRCodeData = qRCodeGenerator.CreateQrCode(item.QRContents, QRCodeGenerator.ECCLevel.Q);
                var qRCode = new QRCode(qRCodeData);
                var bitmap = qRCode.GetGraphic(20);
                bitmap.Save(ms, ImageFormat.Png);

                ViewBag.QRCode = "data:image/png;base64," + Convert.ToBase64String(ms.ToArray());

                return View();
            }
             
        }
        
        
        public IActionResult PrintQRCode(Guid Id)
        {
            var item = _db.Items.Find(Id);

            if(item == null)
            {
                return NotFound();
            }
            else
            {
                var ms = new MemoryStream();
                var qRCodeGenerator = new QRCodeGenerator();
                var qRCodeData = qRCodeGenerator.CreateQrCode(item.QRContents, QRCodeGenerator.ECCLevel.Q);
                var qRCode = new QRCode(qRCodeData);
                var bitmap = qRCode.GetGraphic(20);
                bitmap.Save(ms, ImageFormat.Png);
                bitmap.Save("C:/Users/Public/Pictures/BeanBagItemQRCode.png");

                //ViewBag.QRCode = "data:image/png;base64," + Convert.ToBase64String(ms.ToArray());

                return LocalRedirect("/Inventory/ViewItems?InventoryId=" + item.inventoryId.ToString());
            }
             
        }

        /*public bool GenerateQrCode(string itemId)
        {
            if (itemId == null)
            {
                throw new Exception("QRCode generation failed. ItemID input is null.");
            }

            else if (itemId.Length != 36)
            {
                throw new Exception("QRCode generation failed. ItemID string length is invalid.");
            }

            // Query from db here
            // Throw exception if item not found
            // Throw new Exception("QRCode generation failed. ItemID not found in database.");
            const string itemName = "Item: Munashe's Chair\n";
            const string itemInventory = "Inventory: Furniture Inventory\n";
            const string iType = "Type: Furniture\n";
            itemId = itemName + itemInventory + iType;

            var ms = new MemoryStream();
            var qRCodeGenerator = new QRCodeGenerator();
            var qRCodeData = qRCodeGenerator.CreateQrCode(itemId, QRCodeGenerator.ECCLevel.Q);
            var qRCode = new QRCode(qRCodeData);
            var bitmap = qRCode.GetGraphic(20);

            // Funcionality to put QR Code into its own file in a directory.
            bitmap.Save("C:/Users/Public/Pictures/ItemQRCode.png");

            // As of now the QR Code save on a local dir is done same time as generation
            // but at a later stage will be done in a seperate function. 
            bitmap.Save(ms, ImageFormat.Png);
            ViewBag.QRCode = "data:image/png;base64," + Convert.ToBase64String(ms.ToArray());

            if (ViewBag.QRCode == null)
            {
                return false;
            }

            _qrModel.QrCodeNumber = qRCode.ToString();
            CoupleQrCode(itemId, _qrModel.QrCodeNumber);

            return true;
        }*/


        /*private QrCodeModel _qrModel = new QrCodeModel();

        //---------------------------------- SERVICE CONTRACTS----------------------------------------- 

        *//* This function generates a QRCode and gets the itemDetails as the parameter. Memory stream helps write from
         * and to the memory. Data and input text is used to generate the QR Code's actual 'DATA'. A new instance of
         * the QR code compiled data of text and string input is created.bitmap maps our string and data to image bits
         * which are then 'drawn', thereafter the generated QRCode variable is set.*//*
        public bool GenerateQrCode(string itemId)
        {
            if (itemId == null)
            {
                throw new Exception("QRCode generation failed. ItemID input is null.");
            }

            else if (itemId.Length != 36)
            {
                throw new Exception("QRCode generation failed. ItemID string length is invalid.");
            }

            // Query from db here
            // Throw exception if item not found
            // Throw new Exception("QRCode generation failed. ItemID not found in database.");
            const string itemName = "Item: Munashe's Chair\n";
            const string itemInventory = "Inventory: Furniture Inventory\n";
            const string iType = "Type: Furniture\n";
            itemId = itemName + itemInventory + iType;

            var ms = new MemoryStream();
            var qRCodeGenerator = new QRCodeGenerator();
            var qRCodeData = qRCodeGenerator.CreateQrCode(itemId, QRCodeGenerator.ECCLevel.Q);
            var qRCode = new QRCode(qRCodeData);
            var bitmap = qRCode.GetGraphic(20);

            // Funcionality to put QR Code into its own file in a directory.
            bitmap.Save("C:/Users/Public/Pictures/ItemQRCode.png");

            // As of now the QR Code save on a local dir is done same time as generation
            // but at a later stage will be done in a seperate function. 
            bitmap.Save(ms, ImageFormat.Png);
            ViewBag.QRCode = "data:image/png;base64," + Convert.ToBase64String(ms.ToArray());

            if (ViewBag.QRCode == null)
            {
                return false;
            }

            _qrModel.QrCodeNumber = qRCode.ToString();
            CoupleQrCode(itemId, _qrModel.QrCodeNumber);

            return true;
        }

        //This function couples up the item ID with the QRCode.
        public bool CoupleQrCode(string itemId, string qrNumber)
        {
            // Random generator = new Random();
            // Create a random 6 digit string 
            // String r = generator.Next(0, 1000000).ToString("D6");
            // itemId = qrNumber;        //THIS SHOULD BE FIXED ONCE THE DB IS DONE
            // Boolean to check if the coupling was successful, used as return 
            bool successCode = itemId != null || qrNumber != null;
            // Should return false if coupling never occurs 
            return successCode;
        }

        *//*  
          // This function denotes the functionality needed to print a QR code to a pdf. 
          // It takes in an itemID as a parameter and through that the QR code is retrieved and printed
          public bool printQRCode(string itemId)
          {
              bool printsuccess = false;

              // Query from db here
              // Throw exception if item not found
              // Throw new Exception("QRCode generation failed. ItemID not found in database.");
              const string itemName = "Item: Chair\n";
              const string itemInventory = "Inventory: Furniture Inventory\n";
              const string iType = "Type: Furniture\n";
              itemId = itemName + itemInventory + iType;

              var ms = new MemoryStream();
              var qRCodeGenerator = new QRCodeGenerator();
              var qRCodeData = qRCodeGenerator.CreateQrCode(itemId, QRCodeGenerator.ECCLevel.Q);
              var qRCode = new QRCode(qRCodeData);
              var bitmap = qRCode.GetGraphic(20);

              // Funcionality to put QR Code into its own file in a directory.
              if(printsuccess == false)
              {
                  bitmap.Save("C:/Users/Public/Pictures/ItemQRCode.png");
                  printsuccess == true; 
              }
              return printsuccess
          }
          *//*

        //---------------------------------- VIEW RESPONSE----------------------------------------- 

        // This function returns the QRCode Index page.
        public IActionResult Index()
        {
            return View();
        }

        // This function is used to generate and respond with the QR-code for an item to the View.
        [HttpPost]
        public IActionResult Index(string itemId)
        {
            _qrModel = new QrCodeModel();

            // Mock id until integrate.
            GenerateQrCode(new Guid().ToString());
            return View();
        }*/

    }


}
