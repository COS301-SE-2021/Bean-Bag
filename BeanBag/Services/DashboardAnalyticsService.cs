﻿using System;
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
            if (id == null)
            {
                throw new  Exception("Inventory ID is null.") ;
            }
            
            var idd =  new Guid(id); 
            if (db.Inventories.Find(idd) == null)
            {
                throw new  Exception("Inventory with the given Inventory ID does not exist.") ;
            }
            
            var result = from i in db.Items where i.inventoryId.Equals(idd) select new { i.name, i.type, i.imageURL, i.QRContents, i.price, i.entryDate , i.quantity};
            var res= result.OrderByDescending(d => d.entryDate);
            
            return res;
        }
        
        //Gets the total items added by the user given the inventory id in the functions parameter 
        public int GetTotalItems(string id)
        {
            if (id == null)
            {
                throw new  Exception("Inventory ID is null.") ;
            }

            var idd =  new Guid(id);
            if (db.Inventories.Find(idd) == null)
            {
                throw new  Exception("Inventory with the given Inventory ID does not exist.") ;
            }
            var res = (from i in db.Items where i.inventoryId.Equals(idd) select new {i.quantity}).ToList();
            return res.Sum(t => t.quantity);
            
        }

        //Gets the top items with the highest occurrence in the database
        public  IQueryable GetTopItems(string id)
        {
            if (id == null)
            {
                throw new  Exception("Inventory ID is null.") ;
            }
            
            var idd =  new Guid(id);
            if (db.Inventories.Find(idd) == null)
            {
                throw new  Exception("Inventory with the given Inventory ID does not exist.") ;
            }

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
           
           if (id == null)
           {
               throw new  Exception("Inventory ID is null.") ;
           }else if (time == null)
           {
               throw new Exception("Time period is null");
           }
           
           Guid newId = new Guid(id);
           if (db.Inventories.Find(newId) == null)
           {
               throw new  Exception("Inventory with the given Inventory ID does not exist.") ;
           }
           
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
                   throw new Exception("Invalid timespan given as input, expecting Y, M, W or D") ;
           }
           
           var contents = from cnt in db.Items
               where cnt.inventoryId == newId & cnt.entryDate >= currentDate
               select new { cnt.quantity };

           int sum = 0;
           foreach (var y in contents)
           {
                   sum += y.quantity;
           }
           return sum;
       }
       
       //Gets total items sold in all the inventories 

       public int GetItemsSold(string id, string time)
       {
           if (id == null)
           {
               throw new  Exception("Inventory ID is null.") ;
           }else if (time == null)
           {
               throw new Exception("Time period is null");
           }

           Guid newId = new Guid(id);
            
           if (db.Inventories.Find(newId) == null)
           {
               throw new  Exception("Inventory with the given Inventory ID does not exist.") ;
           }
           
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

           int sum = 0;
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
           if (id == null)
           {
               throw new  Exception("Inventory ID is null.") ;
           }else if (time == null)
           {
               throw new Exception("Time period is null");
           }

           Guid newId = new Guid(id);
           if (db.Inventories.Find(newId) == null)
           {
               throw new  Exception("Inventory with the given Inventory ID does not exist.") ;
           }

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

           double sum = 0;
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
           if (id == null)
           {
               throw new  Exception("Inventory ID is null.") ;
           }else if (time == null)
           {
               throw new Exception("Time period is null");
           }

           Guid newId = new Guid(id);
           if (db.Inventories.Find(newId) == null)
           {
               throw new Exception("Inventory with the given Inventory ID does not exist.");
           }

           DateTime currentDate;
           DateTime prevDate;

           switch (time)
           {
               //Year
               case "Y": 
                   currentDate = DateTime.UtcNow.Date.AddYears(-1);
                   prevDate = DateTime.UtcNow.Date.AddYears(-2);
                   break;
               //Month
               case "M":
                   currentDate = DateTime.UtcNow.Date.AddMonths(-1);
                   prevDate = DateTime.UtcNow.AddMonths(-2);
                   break;
               //Week
               case "W":
                   currentDate = DateTime.UtcNow.Date.AddDays(-7);
                   prevDate = DateTime.UtcNow.AddDays(-14);
                   break;
               //Day
               case "D":
                   currentDate = DateTime.UtcNow.Date;
                   prevDate = DateTime.UtcNow.AddDays(-1);
                   break;
               default:
                   throw new  Exception("Invalid timespan given as input, expecting Y, M, W or D") ;
           }
           
           double sum = 0;
           double prevSum = 0;

           //Current period
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
           
           //Previous period
           var prevContents = from cnt in db.Items
               where cnt.inventoryId == newId &  prevDate > cnt.entryDate
               select new { cnt.price , cnt.isSold};
           
           foreach (var y in prevContents)
           {
               if (y.isSold)
               {
                   prevSum += y.price;
               }
           }
           
           //Growth calculation 
           
           //Zero division
           if (prevSum == 0)
           {
               return 0; 
           }
           
           double growth = (sum - prevSum) / prevSum * 100;
           double rounded = Math.Round(growth,2);

           return rounded;
       }
       
       //Item revenue percentage growth statistic
       public double ItemsRevenueStat(string id, string time)
       {
           return GetSalesGrowth(id, time);

       }
       
       //Item sold percentage growth statistic
       public double ItemsSoldStat(string id, string time)
       {
           if (id == null)
           {
               throw new  Exception("Inventory ID is null.") ;
           }else if (time == null)
           {
               throw new Exception("Time period is null");
           }
           
           Guid newId = new Guid(id);
           if (db.Inventories.Find(newId) == null)
           {
               throw new Exception("Inventory with the given Inventory ID does not exist.");
           }
          
           DateTime currentDate;
           DateTime prevDate;

           switch (time)
           {
               //Year
               case "Y": 
                   currentDate = DateTime.UtcNow.Date.AddYears(-1);
                   prevDate = DateTime.UtcNow.Date.AddYears(-2);
                   break;
               //Month
               case "M":
                   currentDate = DateTime.UtcNow.Date.AddMonths(-1);
                   prevDate = DateTime.UtcNow.AddMonths(-2);
                   break;
               //Week
               case "W":
                   currentDate = DateTime.UtcNow.Date.AddDays(-7);
                   prevDate = DateTime.UtcNow.AddDays(-14);
                   break;
               //Day
               case "D":
                   currentDate = DateTime.UtcNow.Date;
                   prevDate = DateTime.UtcNow.AddDays(-1);
                   break;
               default:
                   throw new  Exception("Invalid timespan given as input, expecting Y, M, W or D") ;
           }
           
           double sum = 0;
           double prevSum = 0;

           //Current period
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
           
           //Previous period
           var prevContents = from cnt in db.Items
               where cnt.inventoryId == newId &  prevDate > cnt.entryDate
               select new { cnt.quantity , cnt.isSold};
           
           foreach (var y in prevContents)
           {
               if (y.isSold)
               {
                   prevSum += y.quantity;
               }
           }
           
           //Growth calculation 

           //Zero division
           if (prevSum == 0)
           {
               return 0; 
           }
           double growth = (sum - prevSum) / prevSum * 100; 
           double rounded = Math.Round(growth,2);

           return rounded;
       }
       
       //Item revenue percentage growth statistic
       public double ItemAvailableStat(string id, string time)
       {
           if (id == null)
           {
               throw new  Exception("Inventory ID is null.") ;
           }else if (time == null)
           {
               throw new Exception("Time period is null");
           }
           
           Guid newId = new Guid(id);
           if (db.Inventories.Find(newId) == null)
           {
               throw new Exception("Inventory with the given Inventory ID does not exist.");
           }
          
           DateTime currentDate;
           DateTime prevDate;

           switch (time)
           {
               //Year
               case "Y": 
                   currentDate = DateTime.UtcNow.Date.AddYears(-1);
                   prevDate = DateTime.UtcNow.Date.AddYears(-2);
                   break;
               //Month
               case "M":
                   currentDate = DateTime.UtcNow.Date.AddMonths(-1);
                   prevDate = DateTime.UtcNow.AddMonths(-2);
                   break;
               //Week
               case "W":
                   currentDate = DateTime.UtcNow.Date.AddDays(-7);
                   prevDate = DateTime.UtcNow.AddDays(-14);
                   break;
               //Day
               case "D":
                   currentDate = DateTime.UtcNow.Date;
                   prevDate = DateTime.UtcNow.AddDays(-1);
                   break;
               default:
                   throw new  Exception("Invalid timespan given as input, expecting Y, M, W or D") ;
           }
           
           double sum = 0;
           double prevSum = 0;

           //Current period
           var contents = from cnt in db.Items
               where cnt.inventoryId == newId & cnt.entryDate >= currentDate
               select new { cnt.quantity , cnt.isSold};

           foreach (var y in contents)
           {
               sum += y.quantity;
           }
           
           //Previous period
           var prevContents = from cnt in db.Items
               where cnt.inventoryId == newId &  prevDate > cnt.entryDate
               select new { cnt.quantity};
           
           foreach (var y in prevContents)
           {
               {
                   prevSum += y.quantity;
               }
           }
           
           //Growth calculation 
           
           //Zero division
           if (prevSum == 0)
           {
               return 0; 
           }
           
           double growth = (sum - prevSum) / prevSum * 100;
           double rounded = Math.Round(growth,2);

           return rounded;
       }

    }
}