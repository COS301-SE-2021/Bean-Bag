using System;
using System.Linq;
using BeanBag.Database;
using BeanBag.Models;
using BeanBag.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace BeanBagIntegrationTests
{
    public class IntegrationDashboardTest
    {
        
        private readonly DBContext _context;
        private readonly IConfiguration config;
        
        public IntegrationDashboardTest()
        {
            this.config = new ConfigurationBuilder().AddJsonFile("appsettings.local.json").Build();
            
            var serviceProvider = new ServiceCollection()
                .AddEntityFrameworkSqlServer()
                .BuildServiceProvider();

            var builder = new DbContextOptionsBuilder<DBContext>();

            var connString = config.GetValue<string>("Database:DefaultConnection");
            
            builder.UseSqlServer(connString).UseInternalServiceProvider(serviceProvider);

            _context = new DBContext(builder.Options);

        }
        
        //Integration test defined to test the getting the recent items that have been added (positive testing)
        [Fact]
        public void Get_Recent_Items_Valid()
        {
            //ARRANGE
            var dashService = new DashboardAnalyticsService(_context);
            
            var chars = "0123456789";
            var stringChars = new char[5];
            var random = new Random();

            for (var i = 0; i < stringChars.Length; i++)
            {
                stringChars[i] = chars[random.Next(chars.Length)];
            }

            var finalString = new String(stringChars);

            var myGuidEnd = finalString;

            var u2 = finalString.Substring(0, 4);
            var u3 = finalString.Substring(1, 4);
            
            Guid theId2 = new("00000000-0000-0000-0000-0000000" + myGuidEnd);

            Guid itemId1 = new("10000000-0000-0000-0000-00000000"+ u2);
            Guid itemId2 = new("10000000-0000-0000-0000-00000000"+ u3);
            
            var myDay = DateTime.MinValue;
            
            var thenew = new Inventory {Id = theId2, name = "Integration test inventory", userId = u2};
            
            var item1 = new Item { Id = itemId1, name = "Leopard stripe shirt", inventoryId  = theId2, type = "Clothes"};

            var item2 = new Item { Id = itemId2, name = "Leopard stripe shirt", inventoryId  = theId2, type = "Clothes"};

            var invSer = new InventoryService(_context);
            var itemSer = new ItemService(_context);
            
            //ACT
            
            invSer.CreateInventory(thenew);
            itemSer.CreateItem(item1);
            itemSer.CreateItem(item2);
            
            var m = dashService.GetRecentItems(theId2.ToString());
            
            //ASSERT
            Assert.NotNull(m);
            
            itemSer.DeleteItem(itemId1);
            itemSer.DeleteItem(itemId2);
            invSer.DeleteInventory(theId2, u2);
        }

        [Fact]
        public void Get_recent_items_id_null()
        {
            //ARRANGE
            var mySer = new DashboardAnalyticsService(_context);
            string myid = null;
            
            //ACT
            void Act() => mySer.GetRecentItems(myid);
            
            //ASSERT
            var exception = Assert.Throws<Exception>(Act);
            Assert.Equal("Inventory ID is null.", exception.Message);
        }
        
        [Fact]
        public void Get_recent_items_invalid_id()
        {
            //ARRANGE
            var mySer = new DashboardAnalyticsService(_context);
            string myid = "10000000-0000-0000-0000-000000000009";
            
            //ACT
            void Act() => mySer.GetRecentItems(myid);
            
            //ASSERT
            var exception = Assert.Throws<Exception>(Act);
            Assert.Equal("Inventory with the given Inventory ID does not exist.", exception.Message);
        }

        //Integration test defined to test the getting of total items in an invetory (positive testing)
        [Fact]
        public void Get_Total_Items_Valid()
        {
            //ARRANGE
            var dashService = new DashboardAnalyticsService(_context);
            
            var chars = "0123456789";
            var stringChars = new char[5];
            var random = new Random();

            for (var i = 0; i < stringChars.Length; i++)
            {
                stringChars[i] = chars[random.Next(chars.Length)];
            }

            var finalString = new String(stringChars);

            var myGuidEnd = finalString;

            var u2 = finalString.Substring(0, 4);
            var u3 = finalString.Substring(1, 4);
            
            Guid theId2 = new("00000000-0000-0000-0000-0000000" + myGuidEnd);

            Guid itemId1 = new("10000000-0000-0000-0000-00000000"+ u2);
            Guid itemId2 = new("10000000-0000-0000-0000-00000000"+ u3);
            
            var myDay = DateTime.MinValue;

            var thenew = new Inventory {Id = theId2, name = "Integration test inventory", userId = u2};
            
            var item1 = new Item { Id = itemId1, name = "Leopard stripe shirt", inventoryId  = theId2, type = "Clothes", quantity = 2};

            var item2 = new Item { Id = itemId2, name = "Leopard stripe shirt", inventoryId  = theId2, type = "Clothes", quantity = 3};

            var invSer = new InventoryService(_context);
            var itemSer = new ItemService(_context);
            
            //ACT
            
            invSer.CreateInventory(thenew);
            itemSer.CreateItem(item1);
            itemSer.CreateItem(item2);

            var m = dashService.GetTotalItems(theId2.ToString());
            
            //ASSERT
            Assert.Equal(5, m);
            itemSer.DeleteItem(itemId1);
            itemSer.DeleteItem(itemId2);
            invSer.DeleteInventory(theId2, u2);
        }
        
        [Fact]
        public void Get_total_items_id_null()
        {
            //ARRANGE
            var mySer = new DashboardAnalyticsService(_context);
            string myid = null;
            
            //ACT
            void Act() => mySer.GetTotalItems(myid);
            
            //ASSERT
            var exception = Assert.Throws<Exception>(Act);
            Assert.Equal("Inventory ID is null.", exception.Message);
        }
        
        [Fact]
        public void Get_total_items_invalid_id()
        {
            //ARRANGE
            var mySer = new DashboardAnalyticsService(_context);
            string myid = "10000000-0000-0000-0000-000000000009";
            
            //ACT
            void Act() => mySer.GetTotalItems(myid);
            
            //ASSERT
            var exception = Assert.Throws<Exception>(Act);
            Assert.Equal("Inventory with the given Inventory ID does not exist.", exception.Message);
        }
        
        //Integration test defined to test the getting of top items in an inventory (positive testing)
        [Fact]
        public void Get_Top_Items_Valid()
        {
            //ARRANGE
            var dashService = new DashboardAnalyticsService(_context);

            var chars = "0123456789";
            var stringChars = new char[5];
            var random = new Random();

            for (var i = 0; i < stringChars.Length; i++)
            {
                stringChars[i] = chars[random.Next(chars.Length)];
            }

            var finalString = new String(stringChars);

            var myGuidEnd = finalString;

            var u2 = finalString.Substring(0, 4);
            var u3 = finalString.Substring(1, 4);
            
            Guid theId2 = new("00000000-0000-0000-0000-0000000" + myGuidEnd);

            Guid itemId1 = new("10000000-0000-0000-0000-00000000"+ u2);
            Guid itemId2 = new("10000000-0000-0000-0000-00000000"+ u3);
            Guid itemId3 = new("10000000-0000-0000-0500-00000000"+ u3);
            Guid itemId4 = new("10000000-0000-0000-4000-00000000"+ u3);
            
            DateTime myDay = DateTime.MinValue;

            Inventory thenew = new Inventory {Id = theId2, name = "Integration test inventory", userId = u2};
            
            var item1 = new Item { Id = itemId1, name = "Leopard stripe shirt", inventoryId  = theId2, type = "Clothes", quantity = 5};

            var item2 = new Item { Id = itemId2, name = "Leopard stripe shirt", inventoryId  = theId2, type = "Clothes", quantity = 3};
            var item3 = new Item { Id = itemId2, name = "Leopard stripe shirt", inventoryId  = theId2, type = "Furniture", quantity = 3};
            var item4 = new Item { Id = itemId2, name = "Leopard stripe shirt", inventoryId  = theId2, type = "Clothes", quantity = 1};

            var invSer = new InventoryService(_context);
            var itemSer = new ItemService(_context);
            
            //ACT
            
            invSer.CreateInventory(thenew);
            itemSer.CreateItem(item1);
            itemSer.CreateItem(item2);
            
            var m = dashService.GetTopItems(theId2.ToString());
            
            //ASSERT
            Assert.NotNull(m);
            itemSer.DeleteItem(itemId1);
            itemSer.DeleteItem(itemId2);
            invSer.DeleteInventory(theId2, u2);
        }
        
        [Fact]
        public void Get_top_items_id_null()
        {
            //ARRANGE
            var mySer = new DashboardAnalyticsService(_context);
            string myid = null;
            
            //ACT
            void Act() => mySer.GetTopItems(myid);
            
            //ASSERT
            var exception = Assert.Throws<Exception>(Act);
            Assert.Equal("Inventory ID is null.", exception.Message);
        }
        
        [Fact]
        public void Get_top_items_invalid_id()
        {
            //ARRANGE
            var mySer = new DashboardAnalyticsService(_context);
            string myid = "10000000-0000-0000-0000-000000000009";
            
            //ACT
            void Act() => mySer.GetTopItems(myid);
            
            //ASSERT
            var exception = Assert.Throws<Exception>(Act);
            Assert.Equal("Inventory with the given Inventory ID does not exist.", exception.Message);
        }
        
    
        //Integration test defined to test the getting of available items (positive testing)
        [Fact]
        public void Get_Items_Available_Valid()
        {
            //ARRANGE
            var dashService = new DashboardAnalyticsService(_context);

            var chars = "0123456789";
            var stringChars = new char[5];
            var random = new Random();

            for (var i = 0; i < stringChars.Length; i++)
            {
                stringChars[i] = chars[random.Next(chars.Length)];
            }

            var finalString = new String(stringChars);

            var myGuidEnd = finalString;

            var u2 = finalString.Substring(0, 4);
            var u3 = finalString.Substring(1, 4);
            
            Guid theId2 = new("00000000-0000-0000-0000-0000000" + myGuidEnd);

            Guid itemId1 = new("10000000-0000-0000-0000-00000000"+ u2);
            Guid itemId2 = new("10000000-0000-0000-0000-00000000"+ u3);

            var myDay = DateTime.Now;

            var thenew = new Inventory {Id = theId2, name = "Integration test inventory", userId = u2};
            
            var item1 = new Item { Id = itemId1, name = "Leopard stripe shirt", inventoryId  = theId2, type = "Clothes", entryDate = DateTime.Now, quantity = 5};

            var item2 = new Item { Id = itemId2, name = "Leopard stripe shirt", inventoryId  = theId2, type = "Clothes", entryDate = DateTime.Now, quantity = 2};

            var invSer = new InventoryService(_context);
            var itemSer = new ItemService(_context);
            
            //ACT
            
            invSer.CreateInventory(thenew);
            itemSer.CreateItem(item1);
            itemSer.CreateItem(item2);
            
            var m = dashService.GetItemsAvailable(theId2.ToString(), "D");
            var n = dashService.GetItemsAvailable(theId2.ToString(), "W");
            var o = dashService.GetItemsAvailable(theId2.ToString(), "M");
            var p = dashService.GetItemsAvailable(theId2.ToString(), "Y");
            
            //ASSERT
            Assert.InRange(m, 0, Int32.MaxValue);
            Assert.Equal(7, m);
            Assert.Equal(7, n);
            Assert.Equal(7, o);
            Assert.Equal(7, p);
            itemSer.DeleteItem(itemId1);
            itemSer.DeleteItem(itemId2);
            invSer.DeleteInventory(theId2, u2);
            
        }
        
        [Fact]
        public void Get_items_available_id_null()
        {
            //ARRANGE
            var mySer = new DashboardAnalyticsService(_context);
            string myid = null;
            
            //ACT
            void Act() => mySer.GetItemsAvailable(myid, "D");
            
            //ASSERT
            var exception = Assert.Throws<Exception>(Act);
            Assert.Equal("Inventory ID is null.", exception.Message);
        }
        
        
        [Fact]
        public void Get_items_available_time_null()
        {
            //ARRANGE
            var mySer = new DashboardAnalyticsService(_context);
            string myid = "10000000-0000-0000-0000-000000000009";
            
            //ACT
            void Act() => mySer.GetItemsAvailable(myid, null);
            
            //ASSERT
            var exception = Assert.Throws<Exception>(Act);
            Assert.Equal("Time period is null", exception.Message);
        }
        
        [Fact]
        public void Get_items_available_invalid_id()
        {
            //ARRANGE
            var mySer = new DashboardAnalyticsService(_context);
            string myid = "10000000-0000-0000-0000-000000000009";
            
            //ACT
            void Act() => mySer.GetItemsAvailable(myid, "D");
            
            //ASSERT
            var exception = Assert.Throws<Exception>(Act);
            Assert.Equal("Inventory with the given Inventory ID does not exist.", exception.Message);
        }

        //Integration test defined to test the retrieval of items sold (positive testing)
        [Fact]
        public void Get_Items_Sold_Valid()
        {
            //ARRANGE
            var dashService = new DashboardAnalyticsService(_context);

            var chars = "0123456789";
            var stringChars = new char[5];
            var random = new Random();

            for (var i = 0; i < stringChars.Length; i++)
            {
                stringChars[i] = chars[random.Next(chars.Length)];
            }

            var finalString = new String(stringChars);

            var myGuidEnd = finalString;

            var u2 = finalString.Substring(0, 4);
            var u3 = finalString.Substring(1, 4);
            
            Guid theId2 = new("00000000-0000-0000-0000-0000000" + myGuidEnd);

            Guid itemId1 = new("10000000-0000-0000-0000-00000000"+ u2);
            Guid itemId2 = new("10000000-0000-0000-0000-00000000"+ u3);
            
            var myDay = DateTime.Now;

            var thenew = new Inventory {Id = theId2, name = "Integration test inventory", userId = u2};
            
            var item1 = new Item { Id = itemId1, name = "Leopard stripe shirt", inventoryId  = theId2, type = "Clothes", entryDate = DateTime.Now, quantity = 1, isSold = true};

            var item2 = new Item { Id = itemId2, name = "Leopard stripe shirt", inventoryId  = theId2, type = "Clothes", entryDate = DateTime.Now, quantity = 2, isSold = true};

            var invSer = new InventoryService(_context);
            var itemSer = new ItemService(_context);
            
            //ACT
            
            invSer.CreateInventory(thenew);
            itemSer.CreateItem(item1);
            itemSer.CreateItem(item2);
            
            var m = dashService.GetItemsSold(theId2.ToString(), "D");
            var n = dashService.GetItemsSold(theId2.ToString(), "W");
            var o = dashService.GetItemsSold(theId2.ToString(), "M");
            var p = dashService.GetItemsSold(theId2.ToString(), "Y");
            
            //ASSERT
            Assert.InRange(m, 0, Int32.MaxValue);
            Assert.Equal(3, m);
            Assert.Equal(3, n);
            Assert.Equal(3, o);
            Assert.Equal(3, p);
            itemSer.DeleteItem(itemId1);
            itemSer.DeleteItem(itemId2);
            invSer.DeleteInventory(theId2, u2);
        }
        
        [Fact]
        public void Get_items_sold_id_null()
        {
            //ARRANGE
            var mySer = new DashboardAnalyticsService(_context);
            string myid = null;
            
            //ACT
            void Act() => mySer.GetItemsSold(myid, "D");
            
            //ASSERT
            var exception = Assert.Throws<Exception>(Act);
            Assert.Equal("Inventory ID is null.", exception.Message);
        }
        
        
        [Fact]
        public void Get_items_sold_time_null()
        {
            //ARRANGE
            var mySer = new DashboardAnalyticsService(_context);
            string myid = "10000000-0000-0000-0000-000000000009";
            
            //ACT
            void Act() => mySer.GetItemsSold(myid, null);
            
            //ASSERT
            var exception = Assert.Throws<Exception>(Act);
            Assert.Equal("Time period is null", exception.Message);
        }
        
        [Fact]
        public void Get_items_sold_invalid_id()
        {
            //ARRANGE
            var mySer = new DashboardAnalyticsService(_context);
            string myid = "10000000-0000-0000-0000-000000000009";
            
            //ACT
            void Act() => mySer.GetItemsSold(myid, "D");
            
            //ASSERT
            var exception = Assert.Throws<Exception>(Act);
            Assert.Equal("Inventory with the given Inventory ID does not exist.", exception.Message);
        }
        
        [Fact]
        public void Get_items_sold_invalid_time()
        {
            //ARRANGE
            var mySer = new DashboardAnalyticsService(_context);
            string myid = "7A8A7DE5-A390-48BF-5BB0-08D976E02C23";
            
            //ACT
            void Act() => mySer.GetItemsSold(myid, "Z");
            
            //ASSERT
            var exception = Assert.Throws<Exception>(Act);
            Assert.Equal("Invalid timespan given as input, expecting Y, M, W or D", exception.Message);
        }

        //Integration test defined to test the calculation of the revenue from sold items (positive testing)
        [Fact]
        public void Get_Revenue_Valid()
        {
            //ARRANGE
            var dashService = new DashboardAnalyticsService(_context);

            var chars = "0123456789";
            var stringChars = new char[5];
            var random = new Random();

            for (var i = 0; i < stringChars.Length; i++)
            {
                stringChars[i] = chars[random.Next(chars.Length)];
            }

            var finalString = new String(stringChars);

            var myGuidEnd = finalString;

            var u2 = finalString.Substring(0, 4);
            var u3 = finalString.Substring(1, 4);
            
            Guid theId2 = new("00000000-0000-0000-0000-0000000" + myGuidEnd);
            Guid itemId1 = new("10000000-0000-0000-0000-00000000"+ u2);
            Guid itemId2 = new("10000000-0000-0000-0000-00000000"+ u3);

            var myDay = DateTime.Now;
            
            //-----------------------------------------

            var thenew = new Inventory {Id = theId2, name = "Integration test inventory", userId = u2};
            
            var item1 = new Item { Id = itemId1, name = "Leopard stripe shirt", inventoryId  = theId2, type = "Clothes", entryDate = DateTime.Now
                , quantity = 1, isSold = true, price = 100};

            var item2 = new Item { Id = itemId2, name = "Leopard stripe shirt", inventoryId  = theId2, type = "Clothes", entryDate = DateTime.Now
                , quantity = 2, isSold = true, price = 200};

            var invSer = new InventoryService(_context);
            var itemSer = new ItemService(_context);
            
            //ACT
            
            invSer.CreateInventory(thenew);
            itemSer.CreateItem(item1);
            itemSer.CreateItem(item2);
   
            var m = dashService.GetRevenue(theId2.ToString(), "D");
            var n = dashService.GetRevenue(theId2.ToString(), "M");
            var o = dashService.GetRevenue(theId2.ToString(), "W");
            var p = dashService.GetRevenue(theId2.ToString(), "Y");
            
            //ASSERT
            Assert.InRange(m, 0.00, Double.MaxValue);
            Assert.Equal(300, m);
            Assert.Equal(300, n);
            Assert.Equal(300, o);
            Assert.Equal(300, p);
            itemSer.DeleteItem(itemId1);
            itemSer.DeleteItem(itemId2);
            invSer.DeleteInventory(theId2, u2);
        }
        
        [Fact]
        public void Get_revenue_id_null()
        {
            //ARRANGE
            var mySer = new DashboardAnalyticsService(_context);
            string myid = null;
            
            //ACT
            void Act() => mySer.GetRevenue(myid, "D");
            
            //ASSERT
            var exception = Assert.Throws<Exception>(Act);
            Assert.Equal("Inventory ID is null.", exception.Message);
        }
        
        
        [Fact]
        public void Get_revenue_time_null()
        {
            //ARRANGE
            var mySer = new DashboardAnalyticsService(_context);
            string myid = "10000000-0000-0000-0000-000000000009";
            
            //ACT
            void Act() => mySer.GetRevenue(myid, null);
            
            //ASSERT
            var exception = Assert.Throws<Exception>(Act);
            Assert.Equal("Time period is null", exception.Message);
        }
        
        [Fact]
        public void Get_revenue_invalid_id()
        {
            //ARRANGE
            var mySer = new DashboardAnalyticsService(_context);
            string myid = "10000000-0000-0000-0000-000000000009";
            
            //ACT
            void Act() => mySer.GetRevenue(myid, "D");
            
            //ASSERT
            var exception = Assert.Throws<Exception>(Act);
            Assert.Equal("Inventory with the given Inventory ID does not exist.", exception.Message);
        }
        
        [Fact]
        public void Get_revenue_invalid_time()
        {
            //ARRANGE
            var mySer = new DashboardAnalyticsService(_context);
            string myid = "7A8A7DE5-A390-48BF-5BB0-08D976E02C23";
            
            //ACT
            void Act() => mySer.GetRevenue(myid, "Z");
            
            //ASSERT
            var exception = Assert.Throws<Exception>(Act);
            Assert.Equal("Invalid timespan given as input, expecting Y, M, W or D", exception.Message);
        }

        //Integration test defined to test the calculation of sales growth (positive testing)
        [Fact]
        public void Get_Sales_Growth_Valid()
        {
            //ARRANGE
            var dashService = new DashboardAnalyticsService(_context);
            
            var chars = "0123456789";
            var stringChars = new char[5];
            var random = new Random();

            for (var i = 0; i < stringChars.Length; i++)
            {
                stringChars[i] = chars[random.Next(chars.Length)];
            }

            var finalString = new String(stringChars);

            var myGuidEnd = finalString;

            var u2 = finalString.Substring(0, 4);
            var u3 = finalString.Substring(1, 4);
            
            Guid theId2 = new("00000000-0000-0000-0000-0000000" + myGuidEnd);

            Guid itemId1 = new("10000000-0000-0000-0000-00000000"+ u2);
            Guid itemId2 = new("10000000-0000-0000-0000-00000000"+ u3);
            
            var myDay = DateTime.Now;

            var thenew = new Inventory {Id = theId2, name = "Integration test inventory", userId = u2};
            
            var item1 = new Item { Id = itemId1, name = "Leopard stripe shirt", inventoryId  = theId2, type = "Clothes", entryDate = DateTime.Now};

            var item2 = new Item { Id = itemId2, name = "Leopard stripe shirt", inventoryId  = theId2, type = "Clothes", entryDate = DateTime.Now};

            var invSer = new InventoryService(_context);
            var itemSer = new ItemService(_context);
            
            //ACT
            
            invSer.CreateInventory(thenew);
            itemSer.CreateItem(item1);
            itemSer.CreateItem(item2);
            
            var m = dashService.GetSalesGrowth(theId2.ToString(), "D");
            var n = dashService.GetSalesGrowth(theId2.ToString(), "W");
            var o = dashService.GetSalesGrowth(theId2.ToString(), "M");
            var p = dashService.GetSalesGrowth(theId2.ToString(), "Y");
            
            //ASSERT
            Assert.InRange(m, 0, Double.MaxValue);
            Assert.InRange(n, 0, Double.MaxValue);
            Assert.InRange(o, 0, Double.MaxValue);
            Assert.InRange(p, 0, Double.MaxValue);
            itemSer.DeleteItem(itemId1);
            itemSer.DeleteItem(itemId2);
            invSer.DeleteInventory(theId2, u2);
        }
        
        [Fact]
        public void Get_sales_growth_id_null()
        {
            //ARRANGE
            var mySer = new DashboardAnalyticsService(_context);
            string myid = null;
            
            //ACT
            void Act() => mySer.GetSalesGrowth(myid, "D");
            
            //ASSERT
            var exception = Assert.Throws<Exception>(Act);
            Assert.Equal("Inventory ID is null.", exception.Message);
        }
        
        
        [Fact]
        public void Get_sales_growth_time_null()
        {
            //ARRANGE
            var mySer = new DashboardAnalyticsService(_context);
            string myid = "10000000-0000-0000-0000-000000000009";
            
            //ACT
            void Act() => mySer.GetSalesGrowth(myid, null);
            
            //ASSERT
            var exception = Assert.Throws<Exception>(Act);
            Assert.Equal("Time period is null", exception.Message);
        }
        
        [Fact]
        public void Get_sales_growth_invalid_id()
        {
            //ARRANGE
            var mySer = new DashboardAnalyticsService(_context);
            string myid = "10000000-0000-0000-0000-000000000009";
            
            //ACT
            void Act() => mySer.GetSalesGrowth(myid, "D");
            
            //ASSERT
            var exception = Assert.Throws<Exception>(Act);
            Assert.Equal("Inventory with the given Inventory ID does not exist.", exception.Message);
        }
        
        [Fact]
        public void Get_sales_growth_invalid_time()
        {
            //ARRANGE
            var mySer = new DashboardAnalyticsService(_context);
            string myid = "7A8A7DE5-A390-48BF-5BB0-08D976E02C23";
            
            //ACT
            void Act() => mySer.GetSalesGrowth(myid, "Z");
            
            
            //ASSERT
            var exception = Assert.Throws<Exception>(Act);
            Assert.Equal("Invalid timespan given as input, expecting Y, M, W or D", exception.Message);
        }
        
        
        //Integration test defined to test the item revenue calculation from the revenue (positive testing)
        [Fact]
        public void Items_Revenue_Stat_Valid()
        {
     
            //ARRANGE
            var dashService = new DashboardAnalyticsService(_context);
 
            var chars = "0123456789";
            var stringChars = new char[5];
            var random = new Random();

            for (var i = 0; i < stringChars.Length; i++)
            {
                stringChars[i] = chars[random.Next(chars.Length)];
            }

            var finalString = new String(stringChars);

            var myGuidEnd = finalString;

            var u2 = finalString.Substring(0, 4);
            var u3 = finalString.Substring(1, 4);
            
            Guid theId2 = new("00000000-0000-0000-0000-0000000" + myGuidEnd);
            Guid itemId1 = new("10000000-0000-0000-0000-00000000"+ u2);
            Guid itemId2 = new("10000000-0000-0000-0000-00000000"+ u3);

            var myDay = DateTime.Now;

            var thenew = new Inventory {Id = theId2, name = "Integration test inventory", userId = u2};
            
            var item1 = new Item { Id = itemId1, name = "Leopard stripe shirt", inventoryId  = theId2, type = "Clothes", entryDate = DateTime.Now};

            var item2 = new Item { Id = itemId2, name = "Leopard stripe shirt", inventoryId  = theId2, type = "Clothes", entryDate = DateTime.Now};

            var invSer = new InventoryService(_context);
            var itemSer = new ItemService(_context);
            
            //ACT
            
            invSer.CreateInventory(thenew);
            itemSer.CreateItem(item1);
            itemSer.CreateItem(item2);
            
            var m = dashService.ItemsRevenueStat(theId2.ToString(), "W");
            
            //ASSERT
            Assert.InRange(m, 0, Double.MaxValue);
            itemSer.DeleteItem(itemId1);
            itemSer.DeleteItem(itemId2);
            invSer.DeleteInventory(theId2, u2);
        }
        
        [Fact]
        public void Get_item_revenue_stat_id_null()
        {
            //ARRANGE
            var mySer = new DashboardAnalyticsService(_context);
            string myid = null;
            
            //ACT
            void Act() => mySer.ItemsRevenueStat(myid, "D");
            
            //ASSERT
            var exception = Assert.Throws<Exception>(Act);
            Assert.Equal("Inventory ID is null.", exception.Message);
        }
        
        
        [Fact]
        public void Get_item_revenue_time_null()
        {
            //ARRANGE
            var mySer = new DashboardAnalyticsService(_context);
            string myid = "10000000-0000-0000-0000-000000000009";
            
            //ACT
            void Act() => mySer.ItemsRevenueStat(myid, null);
            
            //ASSERT
            var exception = Assert.Throws<Exception>(Act);
            Assert.Equal("Time period is null", exception.Message);
        }
        
        [Fact]
        public void Get_item_revenue_invalid_id()
        {
            //ARRANGE
            var mySer = new DashboardAnalyticsService(_context);
            string myid = "10000000-0000-0000-0000-000000000009";
            
            //ACT
            void Act() => mySer.ItemsRevenueStat(myid, "D");
            
            //ASSERT
            var exception = Assert.Throws<Exception>(Act);
            Assert.Equal("Inventory with the given Inventory ID does not exist.", exception.Message);
        }
        
        [Fact]
        public void Get_item_revenue_invalid_time()
        {
            //ARRANGE
            var mySer = new DashboardAnalyticsService(_context);
            string myid = "7A8A7DE5-A390-48BF-5BB0-08D976E02C23";
            
            //ACT
            void Act() => mySer.ItemsRevenueStat(myid, "Z");
            
            //ASSERT
            var exception = Assert.Throws<Exception>(Act);
            Assert.Equal("Invalid timespan given as input, expecting Y, M, W or D", exception.Message);
        }
        

        //Integration test defined to test the item sold statistic calculations (positive testing)
        [Fact]
        public void Items_Sold_Stat_Valid()
        {
     
            //ARRANGE
            var dashService = new DashboardAnalyticsService(_context);
            
            var chars = "0123456789";
            var stringChars = new char[5];
            var random = new Random();

            for (var i = 0; i < stringChars.Length; i++)
            {
                stringChars[i] = chars[random.Next(chars.Length)];
            }

            var finalString = new String(stringChars);

            var myGuidEnd = finalString;

            var u2 = finalString.Substring(0, 4);
            var u3 = finalString.Substring(1, 4);
            
            Guid theId2 = new("00000000-0000-0000-0000-0000000" + myGuidEnd);
            Guid itemId1 = new("10000000-0000-0000-0000-00000000"+ u2);
            Guid itemId2 = new("10000000-0000-0000-0000-00000000"+ u3);

            var myDay = DateTime.Now;

            var thenew = new Inventory {Id = theId2, name = "Integration test inventory", userId = u2};
            
            var item1 = new Item { Id = itemId1, name = "Leopard stripe shirt", inventoryId  = theId2, type = "Clothes", entryDate = DateTime.Now};

            var item2 = new Item { Id = itemId2, name = "Leopard stripe shirt", inventoryId  = theId2, type = "Clothes", entryDate = DateTime.Now};

            var invSer = new InventoryService(_context);
            var itemSer = new ItemService(_context);
            
            //ACT
            
            invSer.CreateInventory(thenew);
            itemSer.CreateItem(item1);
            itemSer.CreateItem(item2);
            
            var m = dashService.ItemsSoldStat(theId2.ToString(), "D");
            var n = dashService.ItemsSoldStat(theId2.ToString(), "W");
            var o = dashService.ItemsSoldStat(theId2.ToString(), "M");
            var p = dashService.ItemsSoldStat(theId2.ToString(), "Y");
            
            //ASSERT
            Assert.InRange(m, 0, Double.MaxValue);
            Assert.InRange(n, 0, Double.MaxValue);
            Assert.InRange(o, 0, Double.MaxValue);
            Assert.InRange(p, 0, Double.MaxValue);
            itemSer.DeleteItem(itemId1);
            itemSer.DeleteItem(itemId2);
            invSer.DeleteInventory(theId2, u2);
        }
        
                [Fact]
        public void Get_item_sold_stat_id_null()
        {
            //ARRANGE
            var mySer = new DashboardAnalyticsService(_context);
            string myid = null;
            
            //ACT
            void Act() => mySer.GetItemsSold(myid, "D");
            
            //ASSERT
            var exception = Assert.Throws<Exception>(Act);
            Assert.Equal("Inventory ID is null.", exception.Message);
        }
        
        
        [Fact]
        public void Get_item_sold_time_null()
        {
            //ARRANGE
            var mySer = new DashboardAnalyticsService(_context);
            string myid = "10000000-0000-0000-0000-000000000009";
            
            //ACT
            void Act() => mySer.GetItemsSold(myid, null);
            
            //ASSERT
            var exception = Assert.Throws<Exception>(Act);
            Assert.Equal("Time period is null", exception.Message);
        }
        
        [Fact]
        public void Get_item_sold_invalid_id()
        {
            //ARRANGE
            var mySer = new DashboardAnalyticsService(_context);
            string myid = "10000000-0000-0000-0000-000000000009";
            
            //ACT
            void Act() => mySer.GetItemsSold(myid, "D");
            
            //ASSERT
            var exception = Assert.Throws<Exception>(Act);
            Assert.Equal("Inventory with the given Inventory ID does not exist.", exception.Message);
        }
        
        [Fact]
        public void Get_item_sold_invalid_time()
        {
            //ARRANGE
            var mySer = new DashboardAnalyticsService(_context);
            string myid = "7A8A7DE5-A390-48BF-5BB0-08D976E02C23";
            
            //ACT
            void Act() => mySer.GetItemsSold(myid, "Z");
            
            //ASSERT
            var exception = Assert.Throws<Exception>(Act);
            Assert.Equal("Invalid timespan given as input, expecting Y, M, W or D", exception.Message);
        }
        
        //Integration test defined to test the calculation of available item statistics (positive testing)
        [Fact]
        public void Item_Available_Stat_Valid()
        {
            //ARRANGE
            var dashService = new DashboardAnalyticsService(_context);
            
            var chars = "0123456789";
            var stringChars = new char[5];
            var random = new Random();

            for (var i = 0; i < stringChars.Length; i++)
            {
                stringChars[i] = chars[random.Next(chars.Length)];
            }

            var finalString = new String(stringChars);

            var myGuidEnd = finalString;

            var u2 = finalString.Substring(0, 4);
            var u3 = finalString.Substring(1, 4);
            
            Guid theId2 = new("00000000-0000-0000-0000-0000000" + myGuidEnd);
            Guid itemId1 = new("10000000-0000-0000-0000-00000000"+ u2);
            Guid itemId2 = new("10000000-0000-0000-0000-00000000"+ u3);
            
            var myDay = DateTime.Now;

            var thenew = new Inventory {Id = theId2, name = "Integration test inventory", userId = u2};
            
            var item1 = new Item { Id = itemId1, name = "Leopard stripe shirt", inventoryId  = theId2, type = "Clothes", entryDate = DateTime.Now};

            var item2 = new Item { Id = itemId2, name = "Leopard stripe shirt", inventoryId  = theId2, type = "Clothes", entryDate = DateTime.Now};

            var invSer = new InventoryService(_context);
            var itemSer = new ItemService(_context);
            
            //ACT
            
            invSer.CreateInventory(thenew);
            itemSer.CreateItem(item1);
            itemSer.CreateItem(item2);
            
            var m = dashService.ItemAvailableStat(theId2.ToString(), "D");
            var n = dashService.ItemAvailableStat(theId2.ToString(), "W");
            var o = dashService.ItemAvailableStat(theId2.ToString(), "M");
            var p = dashService.ItemAvailableStat(theId2.ToString(), "Y");
            
            //ASSERT
            Assert.InRange(m, 0, Double.MaxValue);
            Assert.InRange(n, 0, Double.MaxValue);
            Assert.InRange(o, 0, Double.MaxValue);
            Assert.InRange(p, 0, Double.MaxValue);
            itemSer.DeleteItem(itemId1);
            itemSer.DeleteItem(itemId2);
            invSer.DeleteInventory(theId2, u2);
        }
        
        [Fact]
        public void Get_item_available_stat_id_null()
        {
            //ARRANGE
            var mySer = new DashboardAnalyticsService(_context);
            string myid = null;
            
            //ACT
            void Act() => mySer.ItemAvailableStat(myid, "D");
            
            //ASSERT
            var exception = Assert.Throws<Exception>(Act);
            Assert.Equal("Inventory ID is null.", exception.Message);
        }
        
        
        [Fact]
        public void Get_item_available_time_null()
        {
            //ARRANGE
            var mySer = new DashboardAnalyticsService(_context);
            string myid = "10000000-0000-0000-0000-000000000009";
            
            //ACT
            void Act() => mySer.ItemAvailableStat(myid, null);
            
            //ASSERT
            var exception = Assert.Throws<Exception>(Act);
            Assert.Equal("Time period is null", exception.Message);
        }
        
        [Fact]
        public void Get_item_available_invalid_id()
        {
            //ARRANGE
            var mySer = new DashboardAnalyticsService(_context);
            string myid = "10000000-0000-0000-0000-000000000009";
            
            //ACT
            void Act() => mySer.ItemAvailableStat(myid, "D");
            
            //ASSERT
            var exception = Assert.Throws<Exception>(Act);
            Assert.Equal("Inventory with the given Inventory ID does not exist.", exception.Message);
        }
        
        [Fact]
        public void Get_item_available_invalid_time()
        {
            //ARRANGE
            var mySer = new DashboardAnalyticsService(_context);
            string myid = "7A8A7DE5-A390-48BF-5BB0-08D976E02C23";
            
            //ACT
            void Act() => mySer.ItemAvailableStat(myid, "Z");
            
            //ASSERT
            var exception = Assert.Throws<Exception>(Act);
            Assert.Equal("Invalid timespan given as input, expecting Y, M, W or D", exception.Message);
        }
        
        
        [Fact]
        public void Get_item_sold2_stat_id_null()
        {
            //ARRANGE
            var mySer = new DashboardAnalyticsService(_context);
            string myid = null;
            
            //ACT
            void Act() => mySer.ItemsSoldStat(myid, "D");
            
            //ASSERT
            var exception = Assert.Throws<Exception>(Act);
            Assert.Equal("Inventory ID is null.", exception.Message);
        }
        
        
        [Fact]
        public void Get_item_sold2_time_null()
        {
            //ARRANGE
            var mySer = new DashboardAnalyticsService(_context);
            string myid = "10000000-0000-0000-0000-000000000009";
            
            //ACT
            void Act() => mySer.ItemsSoldStat(myid, null);
            
            //ASSERT
            var exception = Assert.Throws<Exception>(Act);
            Assert.Equal("Time period is null", exception.Message);
        }
        
        [Fact]
        public void Get_item_sold2_invalid_id()
        {
            //ARRANGE
            var mySer = new DashboardAnalyticsService(_context);
            string myid = "10000000-0000-0000-0000-000000000009";
            
            //ACT
            void Act() => mySer.ItemsSoldStat(myid, "D");
            
            //ASSERT
            var exception = Assert.Throws<Exception>(Act);
            Assert.Equal("Inventory with the given Inventory ID does not exist.", exception.Message);
        }
        
        [Fact]
        public void Get_item_sold2_invalid_time()
        {
            //ARRANGE
            var mySer = new DashboardAnalyticsService(_context);
            string myid = "7A8A7DE5-A390-48BF-5BB0-08D976E02C23";
            
            //ACT
            void Act() => mySer.ItemsSoldStat(myid, "Z");
            
            //ASSERT
            var exception = Assert.Throws<Exception>(Act);
            Assert.Equal("Invalid timespan given as input, expecting Y, M, W or D", exception.Message);
        }

    }
}