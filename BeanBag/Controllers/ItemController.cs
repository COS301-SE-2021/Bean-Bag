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
    /* This controller is used to send and retrieve data to the Item
       views using tenant, inventory, blob storage and AI service functions. */
    public class ItemController : Controller
    {
        // Global variables needed for calling the service classes.
        private readonly IItemService _itemService;
        private readonly IInventoryService _inventoryService;
        private readonly IAIService _aIService;
        private readonly IBlobStorageService _blobStorageService;
        private readonly ITenantService _tenantService;
        // Constructor. 
        public ItemController(IItemService iss, IInventoryService inv, IAIService aI, IBlobStorageService blob, ITenantService ten)
        {
      
            _itemService = iss;
            _inventoryService = inv;
            _aIService = aI;
            _blobStorageService = blob;
            _tenantService = ten;
        }
        
        // This function returns the upload-image view for an item given a unique inventory ID.
        public IActionResult UploadImage(Guid inventoryId)
        {
            List<AIModel> aIModels = _aIService.getAllModels();
            List<SelectListItem> iterationDropDown = new List<SelectListItem>();

            @ViewBag.id = inventoryId;
            foreach (var m in aIModels)
            {
                List<AIModelVersions> iterations = _aIService.getAllAvailableIterationsOfModel(m.Id);
                SelectListGroup tempGroup = new SelectListGroup() { Name = m.name };

                foreach (var i in iterations)
                {
                    iterationDropDown.Add(new SelectListItem
                    {
                        Group = tempGroup,
                        Text = i.Name,
                        Value = i.Id.ToString()
                    });
                }
            }
            ViewBag.iterationDropDown = iterationDropDown;
            return View();
        }

        /* This function takes in an image file to be uploaded into the Azure blob container.
           This method also uses the custom vision AI that will predict what type of item is in the image.*/
        [HttpPost]
        public async Task<IActionResult> UploadImage([FromForm(Name = "file")] IFormFile file, 
            [FromForm(Name = "predictionModel")] string predictionModelId, [FromForm(Name = "id")] string id)
        {
            string imageUrl = await _blobStorageService.uploadItemImage(file);

            // Checking to see if user has selected an AI model to use. Otherwise let them continue as iss
            if (predictionModelId == "None")
            {
                ViewBag.listPredictions = "";
                return LocalRedirect("/Item/Create?imageUrl=" + imageUrl + "&iterationName=null"+"&inventoryId=" +id);
            }
            
            var iteration = _aIService.getIteration(Guid.Parse(predictionModelId)); 
            
            var iterName = iteration.Name.Replace("Version", "Iteration");
            
            ViewBag.listPredictions = _aIService.predict(iteration.projectId, iterName, imageUrl);
            
            return LocalRedirect("/Item/Create?imageUrl=" + imageUrl + "&projectId=" +
                                 iteration.projectId + "&iterationName=" + iterName +"&inventoryId=" +id);
        }

        /* This function is the GET method for creating an item and returns Create View.
           The create page needs to accept an imageURL and item type
           This method is only called once the image of the item is uploaded */
        public IActionResult Create(string imageUrl, string projectId, string iterationName, string inventoryId )
        {
            // This creates a list of the different inventories available to put the item into

            var inventories = _inventoryService.GetInventories(User.GetObjectId());

            IEnumerable < SelectListItem > inventoryDropDown = inventories.Select(i => new SelectListItem
            {
                Text = i.name,
                Value = i.Id.ToString()
            });

            // View bag allows the controller to pass information into the view
            // We are passing the inventoryDrop drown as well as the automated imageURL and
            // itemType from the imageUpload POST method

            ViewBag.InventoryDropDown = inventoryDropDown;
            ViewBag.imageUrl = imageUrl;
            ViewBag.InventoryId = inventoryId;
            if(iterationName != "null")
            {
                Guid id = new Guid(projectId);
                ViewBag.listPredictions = _aIService.predict(id, iterationName, imageUrl);
                ViewBag.usePrediction = true;
            }
            else 
            {
                ViewBag.usePrediction = false;
            }
            return View();
        }

        // This function is the POST method for create.
        // Takes in form from Create view to add a new item to the DB.
        [HttpPost]
        public ActionResult Create(Item newItem,[FromForm(Name ="tags")] string[] tags )
        {
            
            //Check subscription plan before creating an item
            var totalItems = _itemService.GetItems(newItem.inventoryId).Count;
            var subscription = _tenantService.GetCurrentTenant(User.GetObjectId()).TenantSubscription;
      
            if (subscription == "Free")
            {
                if (totalItems >= 15)
                {
                    return Json(new
                    {
                        success = false,
                        message = "You are restricted from adding more items. Update your subscription plan to add more items."
                    });
                }

            }else if (subscription == "Standard")
            {
                if (totalItems >= 75)
                {
                    return Json(new
                    {
                        success = false,
                        message = "You are restricted from adding more items. Update your subscription plan to add more items."
                    });
                }
            }
 
            string type = "";
            //Updates the item type to be a string of tags 
   
            for (int i = 0; i < tags.Length; i++) {

                if (i == tags.Length - 1 || tags[i]== "" || tags[i] == " ") {
                    type += tags[i];
                }
                else
                {
                    type += tags[i] + ",";
                }
            }

            newItem.type = type;
         
            if(!ModelState.IsValid)
            {
                return View(newItem);
            }
            else
            {
                _itemService.CreateItem(newItem);
                
                // Returns back to the viewItems view for the inventory using the inventoryId
            }
            return Json(new { success= true, message = Url.Action("ViewItems", "Inventory",new{InventoryId=newItem.inventoryId}) });
     
        }

        /* This function is the GET method of Edit Item. This returns the view of an item that is being edited
           Accepts an itemId in order to return a view of the item that needs to be edited with it's respective
           information passed along*/
        public IActionResult Edit(Guid id)
        {
            var item = _itemService.FindItem(id);
            
            // Does the item exist in the table 
            if(item == null)
            {
                return NotFound();
            }

            // This creates a list of the different inventories available to put the item into
            var inventories = _inventoryService.GetInventories(User.GetObjectId());

            IEnumerable<SelectListItem> inventoryDropDown = inventories.Select(i => new SelectListItem
            {
                Text = i.name,
                Value = i.Id.ToString()
            });

            // View bag allows the controller to pass information into the view
            ViewBag.InventoryDropDown = inventoryDropDown;
            ViewBag.InventoryId = item.inventoryId;
            ViewBag.imageUrl = item.imageURL;

            //comma separate type
            var items = item.type;
            string[] list = items.Split(",");
            ViewBag.types = list;

            return View(item);
        }

        // This function is the POST method for Edit Item.
       [HttpPost]
        public IActionResult Edit(Item item,[FromForm(Name ="tags")] string[] tags)
        {
            string type = "";
            //Updates the item type to be a string of tags 
   
            for (int i = 0; i < tags.Length; i++) {

                if (i == tags.Length - 1 || tags[i]== "" || tags[i] == " ") {
                    type += tags[i];
                }
                else
                {
                    type += tags[i] + ",";
                }
            }
            item.type = type;
            // Makes sure that 
            if(ModelState.IsValid)
            {
                _itemService.EditItem(item);

                return LocalRedirect("/Inventory/ViewItems?InventoryId=" + item.inventoryId.ToString());
            }
            return Ok(item);
        }

        // This function is the GET method for Delete Item.
        public IActionResult Delete(Guid id)
        {
            var item = _itemService.FindItem(id);
            // Does the item exist in the item table
            if (item == null)
            {
                return NotFound();
            }

            ViewBag.InventoryName = _inventoryService.FindInventory(item.inventoryId).name;
            ViewBag.InventoryId = item.inventoryId;
            return View(item);
        }

        // This function is the POST method for Delete Item.
        [HttpPost]
        public IActionResult DeletePost(Guid id)
        {
            string inventoryId = _itemService.GetInventoryIdFromItem(id).ToString();
            _itemService.DeleteItem(id);

            // Returns back to the view items in inventory using the inventory ID
            // The reason why we can still use item.inventoryId is because it is still an intact variable
            // The item variable is not deleted. Only the information in the item table, not the item variable itself
            return LocalRedirect("/Inventory/ViewItems?InventoryId=" + inventoryId);
        }
        
        // This function allows the user to view a QRCode of an Inventory given the ID by returning an image of it.
        public string ViewQrCode(Guid id)
        {
            var item = _itemService.FindItem(id);

            if(item == null)
            {
                return "No QR Code";
            }
            else
            {
                var ms = new MemoryStream();
                var qRCodeGenerator = new QRCodeGenerator();
                var qRCodeData = qRCodeGenerator.CreateQrCode(item.QRCodeLink, QRCodeGenerator.ECCLevel.Q);
                var qRCode = new QRCode(qRCodeData);
                var bitmap = qRCode.GetGraphic(20);
                bitmap.Save(ms, ImageFormat.Png);

                ViewBag.QRCode = "data:image/png;base64," + Convert.ToBase64String(ms.ToArray());
                ViewBag.InventoryId = item.inventoryId;

                return  ViewBag.QRCode;
            }
        }
        
        // This function allows a user to print a QR Code given a specific Inventory ID.
        public string PrintQrCode(Guid id)
        {
            var item = _itemService.FindItem(id);

            if (item == null)
            {
                return NotFound().ToString();
            }
            else
            {
                var ms = new MemoryStream();
                var qRCodeGenerator = new QRCodeGenerator();
                var qRCodeData = qRCodeGenerator.CreateQrCode(item.QRCodeLink, QRCodeGenerator.ECCLevel.Q);
                var qRCode = new QRCode(qRCodeData);
                var bitmap = qRCode.GetGraphic(20);
                bitmap.Save(ms, ImageFormat.Png);
                bitmap.Save("C:/Users/Public/Pictures/"+id +".png");

                return id.ToString();
            }
        }
    }
}
