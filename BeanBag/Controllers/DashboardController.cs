using System.Collections.Generic;
using System.Linq;
using BeanBag.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Identity.Web;

namespace BeanBag.Controllers
{
    public class DashboardController : Controller
    {
        private readonly IInventoryService _inventoryService;
        private readonly IDashboardAnalyticsService _dashboardAnalyticsService;

        public DashboardController( IInventoryService inv, IDashboardAnalyticsService dash)
        {
            // Inits the db context allowing us to use CRUD operations on the inventory table
            _inventoryService = inv;
            _dashboardAnalyticsService = dash;
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
            var result = _dashboardAnalyticsService.GetRecentItems(id);
            return Json(result);  
        }
    }
}