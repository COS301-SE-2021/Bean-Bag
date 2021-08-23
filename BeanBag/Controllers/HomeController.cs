using System.Collections.Generic;
using System.Linq;
using BeanBag.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Identity.Web;

namespace BeanBag.Controllers
{
    /* This controller is used to send and retrieve data to the dashboard
    view using dashboard analytics service functions. */
    public  class  HomeController : Controller
    {
        // Global variables needed for calling the service classes.
        private readonly IInventoryService _inventoryService;
        private readonly IDashboardAnalyticsService _dashboardAnalyticsService;
        private readonly IItemService _itemService;

        // Constructor.
        public HomeController(IInventoryService inv, IDashboardAnalyticsService dash, IItemService itm)
        { 
            _inventoryService = inv;
            _dashboardAnalyticsService = dash;
            _itemService = itm;
        }

        // This function returns the Index page for the dashboard, returns drop-down-lists for the page view.
        public IActionResult Index()
        {
            //Inventory Drop-Down-List
            var inventories = _inventoryService.GetInventories(User.GetObjectId());
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
            ViewBag.hasItems = true;

            // Checking inventories for an empty state 
            foreach (var t in inventories)
            {
                if (inventories.Count==0 || _itemService.GetItems(t.Id).Count ==0)
                {
                    ViewBag.hasItems = false;
                    break;
                }
            }

            return View();
        }
        
        // This function gets the recent items for recently added items card.
        public JsonResult GetItems(string id)
        {
            var result = _dashboardAnalyticsService.GetRecentItems(id);
            return Json(result);  
        }

        // This function gets the total items for total items card.
        public int TotalItems(string id)
        {
            var result = _dashboardAnalyticsService.GetTotalItems(id);
            return result;
            
        }
        
        // This function gets the  top 3 items for the chart in the total items card.
        public JsonResult TopItems(string id , int total)
        {
            var result = _dashboardAnalyticsService.GetTopItems(id);
            return Json(result);  
        }
        
        // This function gets the  items available for items available card.
        public int ItemsAvailable(string id, string time)
        {
            var result = _dashboardAnalyticsService.GetItemsAvailable(id , time); 
            return result;
        }

        // This function gets the items sold for items card.
        public int ItemsSold(string id, string time)
        {
            var result = _dashboardAnalyticsService.GetItemsSold(id , time); 
            return result;
        }

        //This function gets the revenue over a specific time.
        public double Revenue(string id, string time)
        {
            var result = _dashboardAnalyticsService.GetRevenue(id , time); 
            return result;
        }
        
        //This function gets the sales growth over a specific time.
        public double SalesGrowth(string id, string time)
        {
            var result = _dashboardAnalyticsService.GetSalesGrowth(id , time); 
            return result;
        }
        
            
        // This function gets the item available percentage growth statistic over a specific time.
        public double AvailableStat(string id, string time)
        {
            var result = _dashboardAnalyticsService.ItemAvailableStat(id , time); 
            return result;
        }
        
               
        //This function gets the item sold percentage growth statistic over a specific time.
        public double SoldStat(string id, string time)
        {
            var result = _dashboardAnalyticsService.ItemsSoldStat(id , time); 
            return result;
        }
        
        //This function gets the item revenue percentage growth statistic over a specific time.
        public double RevenueStat(string id, string time)
        {
            var result = _dashboardAnalyticsService.ItemsRevenueStat(id , time); 
            return result;
        }
    }
}