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
       public int GetItemsAvailable(string id, string time)
       {
           Guid newId = new Guid(id);
           int sum = 0;

           DateTime currentDate;

           switch (time)
           {
               //Year
               case "Y": 
                   currentDate = DateTime.UtcNow.Date.AddYears(-1);
                   break;
               //Month
               case "M":
                   currentDate = DateTime.UtcNow.Date.AddMonths(-1);
                   break;
               //Week
               case "W":
                   currentDate = DateTime.UtcNow.Date.AddDays(-7);
                   break;
               //Day
               case "D":
                   currentDate = DateTime.UtcNow.Date;
                   break;
               default:
                   throw new  Exception("Invalid timespan given as input, expecting Y, M, W or D") ;
           }
           
           var contents = from cnt in db.Items
               where cnt.inventoryId == newId & cnt.entryDate >= currentDate
               select new { cnt.quantity };

           foreach (var y in contents)
           {
                   sum += y.quantity;
           }
           return sum;
       }
       
       //Gets total items sold in all the inventories 

       public int GetItemsSold(string id, string time)
       {
           Guid newId = new Guid(id);
           int sum = 0;

           DateTime currentDate;

           switch (time)
           {
               //Year
               case "Y": 
                   currentDate = DateTime.UtcNow.Date.AddYears(-1);
                   break;
               //Month
               case "M":
                   currentDate = DateTime.UtcNow.Date.AddMonths(-1);
                   break;
               //Week
               case "W":
                   currentDate = DateTime.UtcNow.Date.AddDays(-7);
                   break;
               //Day
               case "D":
                   currentDate = DateTime.UtcNow.Date;
                   break;
               default:
                   throw new  Exception("Invalid timespan given as input, expecting Y, M, W or D") ;
           }
           
           var contents = from cnt in db.Items
               where cnt.inventoryId == newId & cnt.entryDate >= currentDate
               select new { cnt.quantity , cnt.isSold};

           foreach (var y in contents)
           {
               if (y.isSold)
               {
                   sum += y.quantity;
               }
           }
           return sum;
       }

       //Get revenue for all inventories 
       public double GetRevenue(string id, string time)
       { 
           Guid newId = new Guid(id);
           double sum = 0;

           DateTime currentDate;

           switch (time)
           {
               //Year
               case "Y": 
                   currentDate = DateTime.UtcNow.Date.AddYears(-1);
                   break;
               //Month
               case "M":
                   currentDate = DateTime.UtcNow.Date.AddMonths(-1);
                   break;
               //Week
               case "W":
                   currentDate = DateTime.UtcNow.Date.AddDays(-7);
                   break;
               //Day
               case "D":
                   currentDate = DateTime.UtcNow.Date;
                   break;
               default:
                   throw new  Exception("Invalid timespan given as input, expecting Y, M, W or D") ;
           }
           
           var contents = from cnt in db.Items
               where cnt.inventoryId == newId & cnt.entryDate >= currentDate
               select new { cnt.price , cnt.isSold};

           foreach (var y in contents)
           {
               if (y.isSold)
               {
                   sum += y.price;
               }
           }
           return sum;
       }
       
       //Get sales growth for all inventories 
       public double GetSalesGrowth(string id, string time)
       {
           //monthly
           
           
           //yearly
           
           
           //weekly 
           
           
           return 0;
       }

    }
}