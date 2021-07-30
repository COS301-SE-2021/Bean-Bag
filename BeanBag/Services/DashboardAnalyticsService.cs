using System;
using System.Linq;
using BeanBag.Database;
using BeanBag.Models;

namespace BeanBag.Services
{
    public class DashboardAnalyticsService :IDashboardAnalyticsService
    {
        private readonly DBContext db;
        private readonly IInventoryService inventoryService;
        private readonly IItemService itemService;
        
        //Constructor
        public DashboardAnalyticsService( DBContext db, IInventoryService inventoryService, IItemService itemService)
        {
            this.db = db;
            this.inventoryService = inventoryService;
            this.itemService = itemService;
        }

        //Gets the recent items added to a specific inventory id in the functions parameter 
        public IOrderedQueryable GetRecentItems(string id)
        {
            var idd =  new Guid(id);
            var result = from i in db.Items where i.inventoryId.Equals(idd) select new { i.name, i.type, i.imageURL, i.QRContents, i.price, i.entryDate , i.quantity};
            var res= result.OrderByDescending(d => d.entryDate);
            
            return res;
        }
        
        //Gets the total items added by the user given the inventory id in the functions parameter 
        public int GetTotalItems(string id)
        {
            var idd =  new Guid(id);
            var res = (from i in db.Items where i.inventoryId.Equals(idd) select new {i.quantity}).ToList();
            return res.Sum(t => t.quantity);
            
        }

        //Gets the top items with the highest occurrence in the database
        public  IQueryable GetTopItems(string id)
        {
            var idd =  new Guid(id);
            var res = (from i in db.Items where i.inventoryId.Equals(idd) select new {i.quantity}).ToList();
            int tot= res.Sum(t => t.quantity);

            var topItems = db.Set<Item>()
                .Where(x => x.inventoryId.Equals(idd))
                .GroupBy(x => x.type)
                .Select(x => new {ProductId = x.Key, QuantitySum = x.Sum(a => a.quantity)})
                .OrderByDescending(x => x.QuantitySum)
                .Take(3)
                .Select(x => new {type = x.ProductId, qsum = x.QuantitySum, total= tot});


            return topItems;
        }
        
        //Gets items available in all the inventories 
       public int GetItemsAvailable(string id)
       {
           var inv = inventoryService.GetInventories(id);
           int sum = 0;
           foreach (var x in inv)
           {
               var items = itemService.GetItems(x.Id);

               foreach (var y in items)
               {
                   sum += y.quantity;
               }
               
           }
           return sum;
       }
       
       //Gets total items sold in all the inventories 

       public int GetItemsSold(string id)
       {
           
           var inv = inventoryService.GetInventories(id);
           int sum = 0;
           foreach (var x in inv)
           {
               var items = itemService.GetItems(x.Id);

               foreach (var y in items)
               {
                   if (y.isSold)
                   {
                       sum += 1;
                   }
               }
           }
           return sum;
       }

       //Get revenue for all inventories 
       public double GetRevenue(string id)
       { 
           var inv = inventoryService.GetInventories(id);
           double sum = 0;
           foreach (var x in inv)
           {
               var items = itemService.GetItems(x.Id);

               foreach (var y in items)
               {
                   if (y.isSold)
                   {
                       sum += y.price;
                   }
               }
           }
           return sum;
       }
       
       //Get sales growth for all inventories 
       public double GetSalesGrowth(string id)
       {
           //monthly
           
           
           //yearly
           
           
           //weekly 
           
           
           return 0;
       }

    }
}