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
            if (ModelState.IsValid)
            {
                _db.Inventories.Add(newInvetory);
                _db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(newInvetory);
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

            string inventoryName = _db.Inventories.Find(InventoryId).name;
            var items = from i in _db.Items where i.inventoryId.Equals(InventoryId) select i;

            ViewBag.InventoryName = inventoryName;

            return View(items);
        }

        public IActionResult AddItem()
        {
            return View();
        }

        public IActionResult Edit(Guid? id)
        {
            if(id == null)
            {
                return NotFound();
            }

            var inventory = _db.Inventories.Find(id);

            if(inventory == null)
            {
                return NotFound();
            }

            return View(inventory);
        }

        [HttpPost]
        //[ValidateAntiForgeryToken]
        public IActionResult EditPost(Inventory inventory)
        {

            if(ModelState.IsValid)
            {
                _db.Inventories.Update(inventory);
                _db.SaveChanges();
                return RedirectToAction("Index");
            }

            return BadRequest();
        }

        public IActionResult Delete(Guid? id)
        {
            if(id == null)
            {
                return NotFound();
            }

            var inventory = _db.Inventories.Find(id);
            if(inventory == null)
            {
                return NotFound();
            }

            return View(inventory);
        }

        [HttpPost]
        //[ValidateAntiForgeryToken]
        public IActionResult DeletePost(Guid? id)
        {
            var inventory = _db.Inventories.Find(id);

            if(inventory == null)
            {
                return NotFound();
            }

            var items = from i in _db.Items where i.inventoryId.Equals(id) select i;
            foreach(var i in items)
            {
                _db.Items.Remove(i);
            }
            _db.Inventories.Remove(inventory);
            _db.SaveChanges();
            return RedirectToAction("Index");      
        }
    }
}
