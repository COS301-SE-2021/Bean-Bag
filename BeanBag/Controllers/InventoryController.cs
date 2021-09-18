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
    // This controller is used to handle any user interaction regarding an inventory.
    public class InventoryController : Controller
    {
        /* This variable is used to interact with the Database/DBContext class.
           Allows us to save, update and delete records */
        private readonly DBContext _db;
        private readonly IInventoryService _inventoryService;
        private readonly ITenantService _tenantService;
        private readonly IPaymentService _paymentService;

        // Constructor.
        public InventoryController(DBContext db, IInventoryService inv,
            ITenantService tenantService, IPaymentService paymentService)
        {
            // Inits the db context allowing us to use CRUD operations on the inventory table
            _db = db;
            _inventoryService = inv;
            _tenantService = tenantService;
            _paymentService = paymentService;
        }

        public void CheckUserRole()
        {
            var user = _db.UserRoles.Find(User.GetObjectId());
            if(user == null)
            {
                user = new UserRoles { userId = User.GetObjectId(), role = "U" };
                _db.UserRoles.Add(user);
                _db.SaveChanges();
            }
        }

        /* This function adds a page parameter, a current sort order parameter, and a current filter
         parameter to the method signature and returns the Inventory Index page view. */
        public IActionResult Index(string sortOrder, string currentFilter, string searchString,
            int? page,DateTime from, DateTime to)
        {
            if(User.Identity is {IsAuthenticated: true})
            {
                
             //A ViewBag property provides the view with the current sort order, because this must be included in 
             //the paging links in order to keep the sort order the same while paging
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


            var model = from s in _inventoryService.GetInventories(User.GetObjectId())
                select s;
                //Search and match data, if search string is not null or empty
                if (!String.IsNullOrEmpty(searchString))
                {
                    model = model.Where(s => s.name.Contains(searchString));
                }

                var inventories = model.ToList();
                switch (sortOrder)
                {
                    case "name_desc":
                        modelList = inventories.OrderByDescending(s => s.name).ToList();
                        break;
                 
                    default:
                        modelList = inventories.OrderBy(s => s.name).ToList();
                        break;
                }

                //Date sorting
                if (sortOrder == "date")
                {
                    modelList =( inventories.Where(t => t.createdDate > from && t.createdDate < to)).ToList();
                }
                
            //indicates the size of list
            int pageSize = 5;
            //set page to one is there is no value, ??  is called the null-coalescing operator.
            int pageNumber = (page ?? 1);
            //return the Model data with paged

            Inventory inventory = new Inventory();
            Pagination viewModel = new Pagination();
            IPagedList<Inventory> pagedList = modelList.ToPagedList(pageNumber, pageSize);
            
            viewModel.Inventory = inventory;
            viewModel.PagedList = pagedList;
            @ViewBag.totalInventories = _inventoryService.GetInventories(User.GetObjectId()).Count;
            
            //Subscription Expired 
            @ViewBag.SubscriptionExpired = false;
            if (_tenantService.GetCurrentTenant(User.GetObjectId()).TenantSubscription != "Free")
            {
                var transaction =
                    _paymentService.GetPaidSubscription(_tenantService.GetCurrentTenant(User.GetObjectId()).TenantId);
                if (transaction.EndDate >= DateTime.Now)
                {
                    @ViewBag.SubscriptionExpired = true;
                }
            }

            //Checking user role is in DB
            CheckUserRole();
            return View(viewModel);
            }
            else
            {
                return LocalRedirect("/");
            }
        }
        
        /* This is function is a post method for creating an inventory, it adds a new inventory
           for the user into the DB and returns the user to the Inventory/Index page. */
        [HttpPost]
        public IActionResult Create(Pagination inventories)
        {
            if(User.Identity is {IsAuthenticated: true})
            {
                inventories.Inventory.userId = User.GetObjectId();
               
                // Checks to see that the newInventory is valid (that the fields filled in the create view are present)
                if (ModelState.IsValid)
                {
                    _inventoryService.CreateInventory(inventories.Inventory);

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

        // This function allows the user to view all of the items within a specified inventory.
        public IActionResult ViewItems(Guid inventoryId, string sortOrder, string currentFilter, string searchString, int? page , DateTime from, DateTime to)
        {
            if(User.Identity is {IsAuthenticated: true})
            {
                
                 //A ViewBag property provides the view with the current sort order, because this must be included in 
                 //  the paging links in order to keep the sort order the same while paging
                ViewBag.CurrentSort = sortOrder;
                ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
                List<Item> modelList;

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


                var model =  from i in _db.Items where i.inventoryId.Equals(inventoryId) select i;
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

                    //Date sorting
                    if (sortOrder == "date")
                    {
                        modelList =( model.Where(t => t.entryDate > from && t.entryDate < to)).ToList();

                    }
           
                //indicates the size of list
                int pageSize = 5;
            
                //set page to one is there is no value, ??  is called the null-coalescing operator.
                int pageNumber = (page ?? 1);
            
                Item items = new Item();
                Pagination viewModel = new Pagination();
                IPagedList<Item> pagedList = modelList.ToPagedList(pageNumber, pageSize);
            
                viewModel.Item = items;
                ViewBag.InventoryName = _inventoryService.FindInventory(inventoryId).name;
                ViewBag.InventoryId= inventoryId;
                viewModel.PagedListItems = pagedList;
                @ViewBag.totalItems = pagedList.Count;

                //Checking user role is in DB
                CheckUserRole();

                //Checking to see if the tenant is allowed to generate reports
                Tenant tenant = _tenantService.GetCurrentTenant(User.GetObjectId());
                if (tenant.TenantSubscription == "Premium")
                    ViewBag.canGenerateReport = true;
                else
                    ViewBag.canGenerateReport = false;

                return View(viewModel);
            }
            else
            {
                return LocalRedirect("/");
            }

        }
        
    /* This function is the GET Method for Edit. This returns the view for editing the information
     related to an inventory. The URL needs to accept the GUID of the inventory that is being edited. */
    public IActionResult Edit(Guid id)
    {
        if(User.Identity is {IsAuthenticated: true})
            {
                // Find the inventory in the inventory table using the inventory ID
                var inventory = _inventoryService.FindInventory(id);

                if(inventory.userId == User.GetObjectId())
                {
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

        /* This function is the POST method for edit inventory. This accepts the inventory model from
         the edit view above. This will allow us to make changes to the respected inventory. */
        [HttpPost]
        public IActionResult Edit(Inventory inventory)
        {
            if(User.Identity is {IsAuthenticated: true})
            {
                // Making sure that the inventory is valid before applying the changes into the DB
                if (ModelState.IsValid)
                {
                    if(_inventoryService.EditInventory(User.GetObjectId(), inventory))
                    {
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        return BadRequest();
                    }                 
                }
                return RedirectToAction("Index");
            }
            else 
            {
                return LocalRedirect("/");
            }
        }

        /* This function is the POST method for delete inventory.
           This allows us to delete an inventory using the inventory ID. */
        [HttpPost]
        public IActionResult DeletePost(Guid id)
        {
            if(User.Identity is {IsAuthenticated: true})
            {
                if(_inventoryService.DeleteInventory(id, User.GetObjectId()))
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
        
        /* This function is the GET method for delete inventory. This returns the view for editing the
         information related to an inventory. The URL needs to accept the GUID of the inventory that 
         is being deleted. */
        public IActionResult Delete(Guid id)
        {
            if(User.Identity is {IsAuthenticated: true})
            {
                // Find the inventory in the inventory table using the inventory ID
                var inventory = _inventoryService.FindInventory(id);
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
    }
}
