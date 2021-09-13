using System.Collections.Generic;
using System.Linq;
using BeanBag.Models;
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
        private readonly ITenantService _tenantService;

        // Constructor.
        public HomeController(IInventoryService inv, IDashboardAnalyticsService dash, IItemService itm ,ITenantService ten)
        { 
            _inventoryService = inv;
            _dashboardAnalyticsService = dash;
            _itemService = itm;
            _tenantService = ten;
        }

        // This function returns the Index page for the dashboard, returns drop-down-lists for the page view.
        public IActionResult Index()
        {
            var inventories = _inventoryService.GetInventories(User.GetObjectId());
            //Reno: Created new inventory for new user
            if (inventories.Count == 0)
            {
                Inventory newInventory = new Inventory()
                {
                    name = "My First Inventory",
                    description = "Give me a description",
                    userId = User.GetObjectId(),
                    createdDate = System.DateTime.Now,
                    publicToTenant = false
                };
                _inventoryService.CreateInventory(newInventory);
            }

            //Thread.Sleep(1000);

            //Inventory Drop-Down-List
            IEnumerable< SelectListItem > inventoryDropDown = inventories.Select(i => new SelectListItem
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

            if (inventories.Count==0)
            {
                ViewBag.hasItems = false;
                return View();
            }

            // Checking inventories for an empty state 
            foreach (var t in inventories)
            {
                if (  _itemService.GetItems(t.Id).Count ==0)
                {
                    ViewBag.hasItems = false;
                    return View();
                }
            }
            var tenant = _tenantService.GetTenantName(_tenantService.GetUserTenantId(User.GetObjectId()));

             ViewBag.TenantName = tenant;

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