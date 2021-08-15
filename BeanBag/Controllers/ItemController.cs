using BeanBag.Database;
using BeanBag.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
        public IActionResult Edit(Item item)
        {
            // Makes sure that 
            if(ModelState.IsValid)
            {
                itemService.EditItem(item);

                return LocalRedirect("/Inventory/ViewItems?InventoryId=" + item.inventoryId.ToString());
            }
            return Ok(item);
            //return LocalRedirect("/Item/Edit?ItemId="+item.Id);
        }

        
       // This is the GET method for delete item
        public IActionResult Delete(Guid id)
        {

            if(User.Identity is {IsAuthenticated: true})
            {

                var item = itemService.FindItem(id);
                // Does the item exist in the item table
                if (item == null)
                {
                    return NotFound();
                }

                ViewBag.InventoryName = inventoryService.FindInventory(item.inventoryId).name;
                ViewBag.InventoryId = item.inventoryId;
            
                return View(item);
                
               // return PartialView("_DeleteItem", viewModel);
            }

            return LocalRedirect("/");
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
        public string ViewQRCode(Guid Id)
        {
            var item = itemService.FindItem(Id);

            if(item == null)
            {
                return "No QR Code";
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

                return  ViewBag.QRCode;
            }
             
        }
        
        
        public string PrintQRCode(Guid Id)
        {
            var item = itemService.FindItem(Id);

            if (item == null)
            {
                return NotFound().ToString();
            }
            else
            {
                var ms = new MemoryStream();
                var qRCodeGenerator = new QRCodeGenerator();
                var qRCodeData = qRCodeGenerator.CreateQrCode(item.QRContents, QRCodeGenerator.ECCLevel.Q);
                var qRCode = new QRCode(qRCodeData);
                var bitmap = qRCode.GetGraphic(20);
                bitmap.Save(ms, ImageFormat.Png);
                bitmap.Save("C:/Users/Public/Pictures/"+Id +".png");

                return Id.ToString();
                //ViewBag.QRCode = "data:image/png;base64," + Convert.ToBase64String(ms.ToArray());
                // return LocalRedirect("/Inventory/ViewItems?InventoryId=" + item.inventoryId.ToString());
            }
             
        }
    }
}
