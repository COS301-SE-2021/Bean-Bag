
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
        public HomeController(IInventoryService inv, IDashboardAnalyticsService dash)
        {
            // Inits the db context allowing us to use CRUD operations on the inventory table
            inventoryService = inv;
            dashboardAnalyticsService = dash;
        }

        //Index page, returns drop-down-lists and the page view
        public IActionResult Index()
        {
            //Inventory Drop-Down-List
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
            
            //TimeFrame Drop-Down-list
            List<SelectListItem> times = new List<SelectListItem>
            {
                new SelectListItem() {Text = "Year", Value = "Y"},
                new SelectListItem() {Text = "Month", Value = "M"},
                new SelectListItem() {Text = "Week", Value = "W"},
                new SelectListItem() {Text = "Day", Value = "D"}
            };

            times.First().Selected = true;
            ViewBag.TimeDropDown = times;

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
        public int ItemsAvailable(string id, string time)
        {
            var result = dashboardAnalyticsService.GetItemsAvailable(id , time); 
            return result;
        }

        //Items sold for items card
        public int ItemsSold()
        {
            var result = dashboardAnalyticsService.GetItemsSold(User.GetObjectId()); 
            return result;
        }
        
        //Revenue 
        public double Revenue()
        {
            var result = dashboardAnalyticsService.GetRevenue(User.GetObjectId()); 
            return result;
        }
        
        //Growth
        public double Growth()
        {
            var result = dashboardAnalyticsService.GetSalesGrowth(User.GetObjectId()); 
            return result;
        }
    }
}