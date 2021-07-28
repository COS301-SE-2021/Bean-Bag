using System;
using System.Collections.Generic;
using System.Linq;
using BeanBag.Database;
using BeanBag.Models;
using BeanBag.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Identity.Web;

namespace BeanBag.Controllers
{
    public class DashboardController : Controller
    {
        private readonly IInventoryService _inventoryService;
        private readonly IItemService _itemService;
        // This variable is used to interact with the Database/DBContext class. Allows us to save, update and delete records
        private readonly DBContext _db;

        public DashboardController(DBContext db, IInventoryService inv, IItemService item)
        {
            // Inits the db context allowing us to use CRUD operations on the inventory table
            _db = db;
            _inventoryService = inv;
            _itemService = item;
        }
        
        // GET
        public IActionResult Index()
        {
            var inventories = _inventoryService.GetInventories(User.GetObjectId());
            IEnumerable < SelectListItem > inventoryDropDown = inventories.Select(i => new SelectListItem
            {
               
                Text = i.name,
                Value = i.Id.ToString()
                
            }
            );
           
          ViewBag.InventoryDropDown = inventoryDropDown;
          return View();
        }

        public JsonResult GetItems(string id)
        {
            var idd =  new Guid(id);
            var result = from i in _db.Items where i.inventoryId.Equals(idd) select new { i.name, i.type, i.imageURL, i.QRContents, i.price, i.entryDate };
            var res= result.OrderByDescending(d => d.entryDate);
            return Json(res);  
        }
    }
}