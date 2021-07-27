using System;
using System.Dynamic;
using BeanBag.Database;
using BeanBag.Models;
using BeanBag.Services;
using Microsoft.AspNetCore.Mvc;
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
            dynamic dy = new ExpandoObject();
            dy.inventories = _inventoryService.GetInventories(User.GetObjectId());
            return View(dy);
        }

        // Get recently added items
        public PartialViewResult GetItems(int x, Guid id)
        {
            dynamic dy = new ExpandoObject();
            dy.item = _itemService.GetItems(id);
            return PartialView("Index");
        }
        
    }
}