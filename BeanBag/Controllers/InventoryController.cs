using BeanBag.Database;
using BeanBag.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BeanBag.Controllers
{
    // This class is used to handle any user interaction regarding an inventory
    public class InventoryController : Controller
    {
        // This variable is used to interact with the Database/DBContext class. Allows us to save, update and delete records 
        private readonly DBContext _db;

        
        public InventoryController(DBContext db)
        {
            _db = db;
        }

        // This is the default view to view all of the inventories associated with a user
        public IActionResult Index()
        {
            IEnumerable<Inventory> inventoryList = _db.Inventories;
            return View(inventoryList);
        }

        // This returns the view from Views/Inventory/Create
        // This is a GET method for create
        public IActionResult Create()
        {
            return View();
        }

        // This is Post method for create
        // Adds a new inventory for the user into the DB
        // Returns the user to the Inventory/Index page
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public IActionResult Create(Inventory newInvetory)
        {
            _db.Inventories.Add(newInvetory);
            _db.SaveChanges();
            return RedirectToAction("Index");
        }

        // This is the Get method for viewItems
        // Views all of the items within the specified inventory
        [HttpGet]
        public IActionResult ViewItems(Guid? InventoryId)
        {
            
            if(InventoryId == null)
            {
                return NotFound();
            }

            var items = from i in _db.Items where i.inventoryId.Equals(InventoryId) select i;
            return View(items);
        }

        public IActionResult AddItem()
        {
            return View();
        }
    }
}
