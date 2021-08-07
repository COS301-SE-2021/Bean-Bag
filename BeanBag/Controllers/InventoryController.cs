using BeanBag.Database;
using BeanBag.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Identity.Web;
using BeanBag.Services;
using X.PagedList;

namespace BeanBag.Controllers
{
   // [Route("api/[controller]")]
    // This class is used to handle any user interaction regarding an inventory
    public class InventoryController : Controller
    {
        // This variable is used to interact with the Database/DBContext class. Allows us to save, update and delete records 
        private readonly DBContext db;

        private readonly IInventoryService inventoryService;

        public InventoryController(DBContext db, IInventoryService inv)
        {
            // Inits the db context allowing us to use CRUD operations on the inventory table
            this.db = db;
            inventoryService = inv;
        }

        public void CheckUserRole()
        {
            var user = db.UserRoles.Find(User.GetObjectId());
            if(user == null)
            {
                user = new UserRoles { userId = User.GetObjectId(), role = "U" };
                db.UserRoles.Add(user);
                db.SaveChanges();
            }
        }

        
         //This code adds a page parameter, a current sort order parameter, and a current filter parameter to the method signature
        public IActionResult Index(string sortOrder, string currentFilter, string searchString, int? page)
        {
            if(User.Identity is {IsAuthenticated: true})
            {

                //A ViewBag property provides the view with the current sort order, because this must be included in 
          //  the paging links in order to keep the sort order the same while paging
            ViewBag.CurrentSort = sortOrder;
            ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            List<Inventory> modelList;

            //ViewBag.CurrentFilter, provides the view with the current filter string.
            //he search string is changed when a value is entered in the text box and the submit button is pressed. In that case, the searchString parameter is not null.
            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewBag.CurrentFilter = searchString;


            var model = from s in inventoryService.GetInventories(User.GetObjectId())
                select s;
                //Search and match data, if search string is not null or empty
                if (!String.IsNullOrEmpty(searchString))
                {
                    model = model.Where(s => s.name.Contains(searchString));
                }
                switch (sortOrder)
                {
                    case "name_desc":
                        modelList = model.OrderByDescending(s => s.name).ToList();
                        break;
                 
                    default:
                        modelList = model.OrderBy(s => s.name).ToList();
                        break;
                }

            
            //indicates the size of list
            int pageSize = 7;
            //set page to one is there is no value, ??  is called the null-coalescing operator.
            int pageNumber = (page ?? 1);
            //return the Model data with paged

            Inventory inventory = new Inventory();
            Pagination viewModel = new Pagination();
            IPagedList<Inventory> pagedList = modelList.ToPagedList(pageNumber, pageSize);
            
            viewModel.Inventory = inventory;
            viewModel.PagedList = pagedList;

            //Checking user role is in DB
            CheckUserRole();

            return View(viewModel);
            }
            else
            {
                return LocalRedirect("/");
            }

           
        }
        
        // This is Post method for create
        // Adds a new inventory for the user into the DB
        // Returns the user to the Inventory/Index page
        [HttpPost]
        public IActionResult Create(Pagination inventories)
        {
            if(User.Identity is {IsAuthenticated: true})
            {
                inventories.Inventory.userId = User.GetObjectId();
               
                // Checks to see that the newInventory is valid (that the fields filled in the create view are present)
                if (ModelState.IsValid)
                {
                    inventoryService.CreateInventory(inventories.Inventory);

                    // Returns back to inventory/index
                     return RedirectToAction("Index");
                }
                // Only goes here if the newInventory is invalid
             
                return RedirectToAction("Index");
            }
            else
            {
                //return LocalRedirect("/");
                return BadRequest();
            }
           
        }

        // This is the Get method for viewItems
        // Views all of the items within the specified inventory
        [HttpGet]
        public IActionResult ViewItems(Guid inventoryId)
        {
            if(User.Identity is {IsAuthenticated: true})
            {
                // Find the inventory in the inventory table using the inventory ID
                Inventory inventory = inventoryService.FindInventory(inventoryId);

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

                // View-bag allows us to pass values from the controller to the respected view
                // Here we are passing the inventory name in order to display it in the viewItems page
                ViewBag.InventoryName = inventoryName;

                // This query gets us all the items inside the respected inventory
                // We pass these items to be displayed into the view
                var items = from i in db.Items where i.inventoryId.Equals(inventoryId) select i;

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
    [HttpGet]
    public IActionResult Edit(Guid id)
    {
        ViewBag.MyRouteId = id;
            if(User.Identity is {IsAuthenticated: true})
            {
                // Find the inventory in the inventory table using the inventory ID
                var inventory = inventoryService.FindInventory(id);

                if(inventory.userId == User.GetObjectId())
                {
                    return View(inventory);
                  
                
                   //return RedirectToAction("Index",inventory);
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
        public IActionResult Edit(Inventory inventory)
        {
            if(User.Identity is {IsAuthenticated: true})
            {
                // Making sure that the inventory is valid before applying the changes into the DB
                if (ModelState.IsValid)
                {
                    if(inventoryService.EditInventory(User.GetObjectId(), inventory))
                    {
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        return BadRequest();
                    }                 
                }

                // If model state is invalid then we return back to the inventory edit view
              //  return View(inventory);
                return RedirectToAction("Index");
            }
            else 
            {
                return LocalRedirect("/");
            }
            
        }

        // This is the GET method for delete inventory
        public IActionResult Delete(Guid id)
        {
            if(User.Identity is {IsAuthenticated: true})
            {

                // Find the inventory in the inventory table using the inventory ID
                var inventory = inventoryService.FindInventory(id);

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
        public IActionResult DeletePost(Guid id)
        {
            if(User.Identity is {IsAuthenticated: true})
            {
                if(inventoryService.DeleteInventory(id, User.GetObjectId()))
                {
                    return RedirectToAction("Index");
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
    }
    
    
    
}
