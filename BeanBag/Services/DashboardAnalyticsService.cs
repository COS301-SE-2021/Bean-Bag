using System;
using System.Linq;
using BeanBag.Database;
using BeanBag.Models;

namespace BeanBag.Services
{
    public class DashboardAnalyticsService :IDashboardAnalyticsService
    {
        private readonly DBContext db;
        
        //Constructor
        public DashboardAnalyticsService( DBContext db)
        {
            this.db = db;
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
           return 0;
       }
    }
}