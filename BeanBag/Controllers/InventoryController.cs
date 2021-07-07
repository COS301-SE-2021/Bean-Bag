using BeanBag.Database;
using BeanBag.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Identity.Web;
using System.Drawing.Imaging;
using System.IO;
using QRCoder;

namespace BeanBag.Controllers
{
    // This class is used to handle any user interaction regarding an inventory
    public class InventoryController : Controller
    {
        // This variable is used to interact with the Database/DBContext class. Allows us to save, update and delete records 
        private readonly DBContext _db;

        public InventoryController(DBContext db)
        {
            // Inits the db context allowing us to use CRUD operations on the inventory table
            _db = db;
        }

        public void checkUserRole()
        {
            var user = _db.UserRoles.Find(User.GetObjectId());
            if(user == null)
            {
                user = new UserRoles { userId = User.GetObjectId(), role = "U" };
                _db.UserRoles.Add(user);
                _db.SaveChanges();
            }
        }

        // This is the default view to view all of the inventories associated with a user
        public IActionResult Index()
        {
            
            // Checks to see if user is logged in
            // If not logged in throw user back to home page
            if(User.Identity.IsAuthenticated)
            {
                string userObjectId = User.GetObjectId();
                var inventories = from i in _db.Inventories where i.userId.Equals(userObjectId) select i;

                //Checking user role is in DB
                checkUserRole();

                return View(inventories);
            }
            else
            {
                return LocalRedirect("/");
            }
            
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
            if(User.Identity.IsAuthenticated)
            {
                newInvetory.userId = User.GetObjectId();
                // Checks to see that the newInventory is valid (that the fields filled in the create view are present)
                if (ModelState.IsValid)
                {
                    // Adds the newInventory to the Inventory table. 
                    _db.Inventories.Add(newInvetory);
                    _db.SaveChanges();

                    // Returns back to inventory/index
                    return RedirectToAction("Index");
                }
                // Only goes here if the newInventory is invalid
                return View(newInvetory);
            }
            else
            {
                return LocalRedirect("/");
            }
           
        }

        // This is the Get method for viewItems
        // Views all of the items within the specified inventory
        [HttpGet]
        public IActionResult ViewItems(Guid? InventoryId)
        {
            if(User.Identity.IsAuthenticated)
            {
                // If the inventory id field in the URL is nothing
                if (InventoryId == null)
                {
                    return NotFound();
                }

                // Find the inventory in the inventory table using the inventory ID
                Inventory inventory = _db.Inventories.Find(InventoryId);

                // If their doesn't exist an inventory with the inventory id given
                if (inventory == null)
                {
                    return NotFound();
                }

                if(inventory.userId != User.GetObjectId())
                {
                    return BadRequest();
                }

                string inventoryName = inventory.name;

                // Viewbag allows us to pass values from the controller to the respected view
                // Here we are passing the inventory name in order to display it in the viewItems page
                ViewBag.InventoryName = inventoryName;

                // This query gets us all the items inside the respected inventory
                // We pass these items to be displayed into the view
                var items = from i in _db.Items where i.inventoryId.Equals(InventoryId) select i;

                return View(items);
            }
            else
            {
                return LocalRedirect("/");
            }
            
        }

        // This is the GET Method for Edit
        // This returns the view for editing the information related to an inventory
        // The URL needs to accept the GUID of the inventory that is being edited
        public IActionResult Edit(Guid? id)
        {
            if(User.Identity.IsAuthenticated)
            {
                // If their doesn't exist an inventory with the inventory id given
                if (id == null)
                {
                    return NotFound();
                }

                // Find the inventory in the inventory table using the inventory ID
                var inventory = _db.Inventories.Find(id);

                if(inventory.userId == User.GetObjectId())
                {
                    if (inventory == null)
                    {
                        return NotFound();
                    }

                    return View(inventory);
                }
                else 
                {
                    return BadRequest();
                }         
            }
            else
            {
                return LocalRedirect("/");
            }
           
        }

        // This is the POST method for edit inventory
        // This accepts the inventory model from the edit view above
        // This will allow us to make changes to the respected inventory
        [HttpPost]
        public IActionResult EditPost(Inventory inventory)
        {
            if(User.Identity.IsAuthenticated)
            {
                // Making sure that the inventory is valid before applying the changes into the DB
                if (ModelState.IsValid)
                {
                    if(inventory.userId == User.GetObjectId())
                    {
                        _db.Inventories.Update(inventory);
                        _db.SaveChanges();
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        return BadRequest();
                    }
                    
                }

                // If model state is invalid then we return back to the inventory edit view
                return View(inventory);
            }
            else 
            {
                return LocalRedirect("/");
            }
            
        }

        // This is the GET method for delete inventory
        public IActionResult Delete(Guid? id)
        {
            if(User.Identity.IsAuthenticated)
            {
                if (id == null)
                {
                    return NotFound();
                }

                // Find the inventory in the inventory table using the inventory ID
                var inventory = _db.Inventories.Find(id);

                if (inventory == null)
                {
                    return NotFound();
                }

                if(inventory.userId != User.GetObjectId())
                {
                    return BadRequest();
                }

                return View(inventory);
            }
            else
            {
                return LocalRedirect("/");
            }
            
        }

        // This is the POSt method for delete inventory
        // This allows us to delete an inventory using the inventory ID
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public IActionResult DeletePost(Guid? id)
        {
            if(User.Identity.IsAuthenticated)
            {
                var inventory = _db.Inventories.Find(id);

                // Checking to see if inventory is in the inventory table
                if (inventory == null)
                {
                    return NotFound();
                }

                if(inventory.userId != User.GetObjectId())
                {
                    return BadRequest();
                }

                // We are removing all the items in the inventory from the items table
                var items = from i in _db.Items where i.inventoryId.Equals(id) select i;
                foreach (var i in items)
                {
                    _db.Items.Remove(i);
                }
                // Deleting the inventory from inventory table
                _db.Inventories.Remove(inventory);
                _db.SaveChanges();
                return RedirectToAction("Index");
            }
            else
            {
                return LocalRedirect("/");
            }
                  
        }
    }
}
