using System;
using System.Linq;
using System.Threading.Tasks;
using BeanBag.AzureSqlDatabase;
using BeanBag.Database;
using BeanBag.Models;
using BeanBag.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Identity.Web;
using X.PagedList;

namespace BeanBag.Controllers
{
    /* This controller is used to send and retrieve data to the account
    view using tenant service and inventory service functions. */
    public class TenantController : Controller
    {
        // Global variables needed for calling the service classes.
        private readonly ITenantService _tenantService;
        private readonly IInventoryService _inventory;
        private readonly IPaymentService _paymentService;
        private IConfiguration _configuration;
        private static string _databaseName;
        private DBContext _dbContext;
        
        private static string _resource;
        private static string _server;

        // Constructor.
        public TenantController(ITenantService tenantService, IInventoryService inventory, IPaymentService paymentService, IConfiguration configuration)
        {
            _tenantService = tenantService;
            _inventory = inventory;
            _paymentService = paymentService;
            
            //Database
            _configuration = new ConfigurationBuilder().SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                .AddJsonFile("appsettings.local.json").Build();
            
            _resource = _configuration.GetValue<String>("AzureAd:Resource");
            _server = _configuration.GetValue<String>("AzureAd:Server");

        }

        /* This function adds a page parameter, a current sort order parameter, and a current filter
        parameter to the method signature and returns the Tenant Index page view. */
         [AllowAnonymous]
        public IActionResult Index(string sortOrder, string currentFilter, string searchString, int? page)
        {
            
            var currentTenant = _tenantService.GetCurrentTenant(User.GetObjectId());
            var dbName = _tenantService.CreateDbName(currentTenant.TenantName);
            
            _dbContext = new DBContext(dbName).GetContext();

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
        public IActionResult CreateTenant(string tenantName, string tenantAddress,
            string tenantEmail, string tenantNumber, string tenantSubscription, string reference, string payId)
        {
            Console.WriteLine("Checking the user id create tenant: " + User.GetObjectId());
            

            if (tenantName == null)
            {
                return RedirectToAction("Index");
            }
            
            
            var id = _tenantService.CreateNewTenant(tenantName, tenantAddress, tenantEmail, tenantNumber,tenantSubscription);
            _tenantService.SignUserUp(User.GetObjectId(), id, User.GetDisplayName());
            var user = _tenantService.GetUser(User.GetObjectId());
            user.UserRole = "A";
                
            var newName = _tenantService.CreateDbName(tenantName);
            _databaseName = newName;

            SelectTenant(id, reference,payId);

            return RedirectToAction("CreateDatabase");
        }

        public async Task<IActionResult> CreateDatabase()
        {
            //Set up SQL manager
            var sqlManager = new AzureDatabaseManager(_resource, _server);
            
            try
            {
                //Create the database
                var created =await sqlManager.CreateDatabase(_databaseName);

                if (created)
                {
                    Console.WriteLine("Database created");
                    //Initialise database
                    var migration = new MigrateToDatabase(_databaseName);
                    migration.ApplyMigration();
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    Console.WriteLine("Database creation unsuccessful");
                }


            }
            catch (Exception e)
            {
                Console.WriteLine("Creation failed");
                
            }

            Console.ReadKey();

            return RedirectToAction("Index", "Home");
        }

        /* This function allows a user to select a tenant and generates
         a new user inventory for demonstration purposes*/
        public IActionResult SelectTenant(string tenant, string reference, string payId)
        {
            if (tenant == null)
            {
                return RedirectToAction("Index");
            }

            //Check if user is new
            var userId = User.GetObjectId();
            var currentTenantId = tenant;
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

                    //confirm transaction
                    if (_tenantService.GetCurrentTenant(userId).TenantSubscription != "Free")
                    {
                        float amount;
                        if (_tenantService.GetCurrentTenant(userId).TenantSubscription.Equals("Standard"))
                        {
                            amount = 500;
                        }
                        else
                        {
                            amount = 1000;
                        }
                        
                        //Add the transaction to the DB 
                        _paymentService.AddTransaction(reference, payId, currentTenantId, amount);


                    }
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