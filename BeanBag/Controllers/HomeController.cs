
using System.Collections.Generic;
using System.Linq;
using BeanBag.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Identity.Web;

namespace BeanBag.Controllers
{
    public  class  HomeController : Controller
    {
        private readonly IInventoryService inventoryService;
        private readonly IDashboardAnalyticsService dashboardAnalyticsService;

        //Constructor
        public HomeController( IInventoryService inv, IDashboardAnalyticsService dash)
        {
            // Inits the db context allowing us to use CRUD operations on the inventory table
            inventoryService = inv;
            dashboardAnalyticsService = dash;
        }

        //Index page, returns drop-down-lists and the page view
        public IActionResult Index()
        {
            var inventories = inventoryService.GetInventories(User.GetObjectId());
            IEnumerable < SelectListItem > inventoryDropDown = inventories.Select(i => new SelectListItem
                {
                    Text = i.name,
                    Value = i.Id.ToString()
                }
            );
            if(inventories.Count!=0)
            {
                inventoryDropDown.First().Selected = true;
            }
          
            

            ViewBag.InventoryDropDown = inventoryDropDown;
            return View();
        }
        
        // Get recent items for recently added items card
        public JsonResult GetItems(string id)
        {
            var result = dashboardAnalyticsService.GetRecentItems(id);
            return Json(result);  
        }

        //Get total items for total items card
        public int TotalItems(string id)
        {
            var result = dashboardAnalyticsService.GetTotalItems(id);
            return result;
            
        }
        
        //Get top 3 items for the chart in the total items card
        public JsonResult TopItems(string id , int total)
        {
            var result = dashboardAnalyticsService.GetTopItems(id);
            return Json(result);  
        }
        
        //Get items available for items available card
        public int ItemsAvailable(string id)
        {
            var result = dashboardAnalyticsService.GetItemsAvailable(id);
            return result;
            
        }

        
    }
}