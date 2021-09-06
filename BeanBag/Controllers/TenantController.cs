using System;
using System.Linq;
using BeanBag.Models;
using BeanBag.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Web;
using X.PagedList;

namespace BeanBag.Controllers
{
    /* This controller is used to send and retrieve data to the account
    view using tenant service and inventory service functions. */
    public class TenantController : Controller
    {
        // Global variables needed for calling the service classes.
        private readonly TenantService _tenantService;
        private readonly IInventoryService _inventory;

        // Constructor.
        public TenantController(TenantService tenantService, IInventoryService inventory)
        {
            _tenantService = tenantService;
            _inventory = inventory;
        }

        /* This function adds a page parameter, a current sort order parameter, and a current filter
        parameter to the method signature and returns the Tenant Index page view. */
         [AllowAnonymous]
        public IActionResult Index(string sortOrder, string currentFilter, string searchString, int? page)
        {
            if (User.GetObjectId() == null)
            {
                throw new Exception("User ID is null");
            }
            else if (_tenantService.SearchUser(User.GetObjectId()))
            {
                return RedirectToAction("Index", "Home");
            }
            
             //A ViewBag property provides the view with the current sort order, because this must be included in 
             //the paging links in order to keep the sort order the same while paging
            ViewBag.CurrentSort = sortOrder;
            ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";

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


            var model = from s in _tenantService.GetTenantList()
                select s;
                //Search and match data, if search string is not null or empty
                if (!String.IsNullOrEmpty(searchString))
                {
                    model = model.Where(s => s.TenantName.Contains(searchString));
                }

                var tenants = model.ToList();
            
                //Sort card list of tenants alphabetically
                var modelList = tenants.OrderBy(s => s.TenantName).ToList();

                
            //indicates the size of list 4 card items per page
            int pageSize = 3;
            //set page to one is there is no value, ??  is called the null-coalescing operator.
            int pageNumber = (page ?? 1);
            //return the Model data with paged

            Tenant tenant = new Tenant();
            Pagination viewModel = new Pagination();
            IPagedList<Tenant> pagedList = modelList.ToPagedList(pageNumber, pageSize);
            
            viewModel.Tenant = tenant;
            viewModel.PagedListTenants = pagedList;
     
            return View(viewModel);
         
        }
        
        // This function allows a user to create a new tenant.
        [HttpPost]
        public IActionResult CreateTenant(string tenantName, string tenantAddress, string tenantEmail, string tenantNumber, string tenantSubscription)
        {
            Console.WriteLine("Checking the user id create tenant: " + User.GetObjectId());


            if (tenantName == null)
            {
                return RedirectToAction("Index");
            }
            else
            {
                _tenantService.CreateNewTenant(tenantName, tenantAddress, tenantEmail, tenantNumber,tenantSubscription); 
            }

            return SelectTenant(tenantName);
        }

        /* This function allows a user to select a tenant and generates
         a new user inventory for demonstration purposes*/
     
        public IActionResult SelectTenant(string tenant)
        {
            if (tenant == null)
            {
                return RedirectToAction("Index");
            }

            //Check if user is new
            var userId = User.GetObjectId();
            var currentTenantName = tenant;
            var currentTenantId = _tenantService.GetTenantId(currentTenantName);
            var userName = "";
            
            if (User.Identity != null)
            {
                userName = User.Identity.Name;
            }
            Console.WriteLine("Checking the user id select tenant: " + User.GetObjectId());

            if (_tenantService.SearchUser(userId) == false)
            {
                 //User is new - add user to database
                //Verify tenant
                if (_tenantService.SearchTenant(currentTenantId))
                {
                    //Verified
                    _tenantService.SignUserUp(userId, currentTenantId, userName);
                }
                else
                {
                    throw new Exception("Tenant does not exist");
                }
            }

            //New inventory created for first time user.
            Inventory newInventory = new Inventory()
            {
                name = "My First Inventory",
                userId = userId,
                description = "My first ever inventory to add new items to",
                createdDate = DateTime.Now
            };
            _inventory.CreateInventory(newInventory);
            
            //logo for first time user 
     
            return RedirectToAction("Index", "Home");
        }
    }
}