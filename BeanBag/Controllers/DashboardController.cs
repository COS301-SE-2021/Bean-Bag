using System.Dynamic;
using BeanBag.Database;
using BeanBag.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Web;

namespace BeanBag.Controllers
{
    public class DashboardController : Controller
    {
        private readonly IInventoryService _inventoryService;
        // This variable is used to interact with the Database/DBContext class. Allows us to save, update and delete records 
        private readonly DBContext _db;

        public DashboardController(DBContext db, IInventoryService _is)
        {
            // Inits the db context allowing us to use CRUD operations on the inventory table
            _db = db;
            _inventoryService = _is;
        }
        
        // GET
        public IActionResult Index()
        {
            dynamic dy = new ExpandoObject();
            dy.inventories = _inventoryService.GetInventories(User.GetObjectId());
            return View(dy);
        }

    
        
    }
}