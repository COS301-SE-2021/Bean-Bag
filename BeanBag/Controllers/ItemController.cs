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
using Microsoft.Identity.Web;
using System.Drawing.Imaging;
using QRCoder;
using BeanBag.Services;

namespace BeanBag.Controllers
{
    public class ItemController : Controller
    {
        private readonly IItemService itemService;
        private readonly IInventoryService inventoryService;
        private readonly IAIService aIService;
        private readonly IBlobStorageService blobStorageService;

        // This variable is used to interact with the Database/DBContext class. Allows us to save, update and delete records 
        //private readonly DBContext _db;
        // The connection string is exposed. Will need to figure out a way of getting it out of appsetting.json. Maybe init in startup like db context ?
        
        public ItemController(DBContext db, IItemService _is, IInventoryService _invs, IAIService _aI, IBlobStorageService _blob)
        {
            //_db = db;
            itemService = _is;
            inventoryService = _invs;
            aIService = _aI;
            blobStorageService = _blob;
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
            string imageURL = await blobStorageService.uploadItemImage(file);
            string prediction = aIService.predict(imageURL);

            return LocalRedirect("/Item/Create?imageUrl="+ imageURL + "&itemType="+ prediction);
        }

        // Get method for create
        // Returns Create view
        // The create page needs to accept an imageURL and item type
        // This method is only called once the image of the item is uploaded
        public IActionResult Create(string imageUrl, string itemType)
        {
            // This creates a list of the different inventories available to put the item into

            var inventories = inventoryService.GetInventories(User.GetObjectId());

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
            if(ModelState.IsValid)
            {
                itemService.CreateItem(newItem);
                
                // Returns back to the viewItems view for the inventory using the inventoryId
                return LocalRedirect("/Inventory/ViewItems?InventoryId="+newItem.inventoryId.ToString());
            }
            // Redirect to create view if the item model is invalid
            return View(newItem);
        }

        // This is the GET method of item edit
        // This returns the view of an item that is being edited
        // Accepts an itemId in order to return a view of the item that needs to be edited with it's respective information passed along
        public IActionResult Edit(Guid Id)
        {
            var item = itemService.FindItem(Id);
            // Does the item exist in the table 
            if(item == null)
            {
                return NotFound();
            }

            // This creates a list of the different inventories available to put the item into
            var inventories = inventoryService.GetInventories(User.GetObjectId());

            IEnumerable<SelectListItem> InventoryDropDown = inventories.Select(i => new SelectListItem
            {
                Text = i.name,
                Value = i.Id.ToString()
            });

            // View bag allows the controller to pass information into the view
            ViewBag.InventoryDropDown = InventoryDropDown;
            ViewBag.InventoryId = item.inventoryId;
            ViewBag.imageUrl = item.imageURL;

            return View(item);
        }

        // This is the POST method for edit Item
        [HttpPost]
        public IActionResult EditPost(Item item)
        {
            // Makes sure that 
            if(ModelState.IsValid)
            {
                itemService.EditItem(item);

                return LocalRedirect("/Inventory/ViewItems?InventoryId=" + item.inventoryId.ToString());
            }

            return View();
        }

        // This is the GET method for delete item
        public IActionResult Delete(Guid Id)
        {
            var item = itemService.FindItem(Id);
            // Does the item exist in the item table
            if (item == null)
            {
                return NotFound();
            }

            ViewBag.InventoryName = inventoryService.FindInventory(item.inventoryId).name;
            return View(item);
        }

        // This is the POST method for delete item
        [HttpPost]
        public IActionResult DeletePost(Guid Id)
        {
            string inventoryId = itemService.GetInventoryIdFromItem(Id).ToString();
            itemService.DeleteItem(Id);

            // Returns back to the view items in inventory using the inventory ID
            // The reason why we can still use item.inventoryId is because it is still an intact variable
            // The item variable is not deleted. Only the information in the item table, not the item variable itself
            return LocalRedirect("/Inventory/ViewItems?InventoryId=" + inventoryId);
        }

        // Define a function to generate a QR code every time we want to view it
        public IActionResult ViewQRCode(Guid Id)
        {
            var item = itemService.FindItem(Id);

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
                ViewBag.InventoryId = item.inventoryId;

                return View();
            }
             
        }
        
        
        public IActionResult PrintQRCode(Guid Id)
        {
            var item = itemService.FindItem(Id);

            if (item == null)
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
