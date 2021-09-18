using System;
using System.Linq;
using BeanBag.Database;
using Microsoft.EntityFrameworkCore;
using Xunit;
using BeanBag.Models;
using BeanBag.Services;


namespace BeanBagUnitTests
{
    public class UnitDashboardTestsConcrete : UnitInventoryTests
    {
        public UnitDashboardTestsConcrete() : base(new DbContextOptionsBuilder<DBContext>()
            .UseSqlite("Filename=Test.db").Options)
        {
            
        }
        
        //Unit test defined for getting the recent items from the person's inventory
        [Fact]
        public void Get_Recent_Items_valid()
        {
            using (var context = new DBContext(ContextOptions))
            {
                //ARRANGE
                var dashService = new DashboardAnalyticsService(context);

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

                Guid itemId1 = new("10000000-0000-0000-0000-00000000" + u2);
                Guid itemId2 = new("10000000-0000-0000-0000-00000000" + u3);

                var myDay = DateTime.MinValue;

                var thenew = new Inventory {Id = theId2, name = "Integration test inventory", userId = u2};

                var item1 = new Item
                    {Id = itemId1, name = "Balenciagas, red", inventoryId = theId2, type = "Clothes"};

                var item2 = new Item
                    {Id = itemId2, name = "Nike black air forces", inventoryId = theId2, type = "Clothes"};

                var invSer = new InventoryService(context);
                var itemSer = new ItemService(context);

                //ACT

                invSer.CreateInventory(thenew);
                itemSer.CreateItem(item1);
                itemSer.CreateItem(item2);

                IOrderedQueryable m = null;
                
                m = dashService.GetRecentItems(theId2.ToString());

                //ASSERT
                Assert.NotNull(m);

                itemSer.DeleteItem(itemId1);
                itemSer.DeleteItem(itemId2);
                invSer.DeleteInventory(theId2, u2);
            }
        }

        
        
        [Fact]
        public void Get_recent_items_id_null()
        {
            using (var context = new DBContext(ContextOptions))
            {
                //ARRANGE
                var mySer = new DashboardAnalyticsService(context);
                string myid = null;

                //ACT
                void Act() => mySer.GetRecentItems(myid);

                //ASSERT
                var exception = Assert.Throws<Exception>(Act);
                Assert.Equal("Inventory ID is null.", exception.Message);
            }
        }
        
        [Fact]
        public void Get_recent_items_invalid_id()
        {
            using (var context = new DBContext(ContextOptions))
            {
                //ARRANGE
                var mySer = new DashboardAnalyticsService(context);
                string myid = "10000000-0000-0000-0000-000000000009";

                //ACT
                void Act() => mySer.GetRecentItems(myid);

                //ASSERT
                var exception = Assert.Throws<Exception>(Act);
                Assert.Equal("Inventory with the given Inventory ID does not exist.", exception.Message);
            }
        }
        
        //Unit test defined for getting the total amount of items across all of a person's inventories
        [Fact]
        public void Get_Total_Items()
        {
            using (var context = new DBContext(ContextOptions))
            {
                //ARRANGE
                var dashService = new DashboardAnalyticsService(context);

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

                Guid itemId1 = new("10000000-0000-0000-0000-00000000" + u2);
                Guid itemId2 = new("10000000-0000-0000-0000-00000000" + u3);
                
                var thenew = new Inventory {Id = theId2, name = "Integration test inventory", userId = u2};

                var item1 = new Item
                    {Id = itemId1, name = "Leopard stripe shirt", inventoryId = theId2, type = "Clothes", quantity = 3};

                var item2 = new Item
                    {Id = itemId2, name = "Brown Sandals", inventoryId = theId2, type = "Clothes", quantity = 3};

                var invSer = new InventoryService(context);
                var itemSer = new ItemService(context);

                //ACT

                invSer.CreateInventory(thenew);
                itemSer.CreateItem(item1);
                itemSer.CreateItem(item2);

                var m = dashService.GetTotalItems(theId2.ToString());

                //ASSERT
                Assert.Equal(6, m);
                itemSer.DeleteItem(itemId1);
                itemSer.DeleteItem(itemId2);
                invSer.DeleteInventory(theId2, u2);
            }
        }
        
        [Fact]
        public void Get_total_items_id_null()
        {
            using (var context = new DBContext(ContextOptions))
            {
                //ARRANGE
                var mySer = new DashboardAnalyticsService(context);
                string myid = null;

                //ACT
                void Act() => mySer.GetTotalItems(myid);

                //ASSERT
                var exception = Assert.Throws<Exception>(Act);
                Assert.Equal("Inventory ID is null.", exception.Message);
            }
        }
        
        [Fact]
        public void Get_total_items_invalid_id()
        {
            using (var context = new DBContext(ContextOptions))
            {
                //ARRANGE
                var mySer = new DashboardAnalyticsService(context);
                string myid = "10000000-0000-0000-0000-000000000009";

                //ACT
                void Act() => mySer.GetTotalItems(myid);

                //ASSERT
                var exception = Assert.Throws<Exception>(Act);
                Assert.Equal("Inventory with the given Inventory ID does not exist.", exception.Message);
            }
        }
        
        //Unit test defined to get the top items from a person's inventories based off a quantifier (i.e. price)
        [Fact]
        public void Get_Top_Items()
        {
            using(var context = new DBContext(ContextOptions))
            {
                //ARRANGE
                var dashService = new DashboardAnalyticsService(context);

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
                
                Inventory thenew = new Inventory {Id = theId2, name = "Integration test inventory", userId = u2};
                
                var item1 = new Item { Id = itemId1, name = "White and gold shirt", inventoryId  = theId2, type = "Clothes", quantity = 5};

                var item2 = new Item { Id = itemId2, name = "Blue beach jeans", inventoryId  = theId2, type = "Clothes", quantity = 3};

                var invSer = new InventoryService(context);
                var itemSer = new ItemService(context);
                
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
        }
        
        [Fact]
        public void Get_top_items_id_null()
        {
            using(var context = new DBContext(ContextOptions))
            {
                //ARRANGE
                var mySer = new DashboardAnalyticsService(context);
                string myid = null;
                
                //ACT
                void Act() => mySer.GetTopItems(myid);
                
                //ASSERT
                var exception = Assert.Throws<Exception>(Act);
                Assert.Equal("Inventory ID is null.", exception.Message);
            }
        }
        
        [Fact]
        public void Get_top_items_invalid_id()
        {
            using(var context = new DBContext(ContextOptions))
            {
                //ARRANGE
                var mySer = new DashboardAnalyticsService(context);
                string myid = "10000000-0000-0000-0000-000000000009";
                
                //ACT
                void Act() => mySer.GetTopItems(myid);
                
                //ASSERT
                var exception = Assert.Throws<Exception>(Act);
                Assert.Equal("Inventory with the given Inventory ID does not exist.", exception.Message);
            }
        }
    
        //Unit test defined to get the items available, currently, in a user's database
        [Fact]
        public void Get_Items_Available()
        {
            using(var context = new DBContext(ContextOptions))
            {
                //ARRANGE
                var dashService = new DashboardAnalyticsService(context);

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
                
                var thenew = new Inventory {Id = theId2, name = "Integration test inventory", userId = u2};
                
                var item1 = new Item { Id = itemId1, name = "Khaki shorts", inventoryId  = theId2, type = "Clothes", entryDate = DateTime.Now, quantity = 5};

                var item2 = new Item { Id = itemId2, name = "Ray bans", inventoryId  = theId2, type = "Clothes", entryDate = DateTime.Now, quantity = 2};

                var invSer = new InventoryService(context);
                var itemSer = new ItemService(context);
                
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
        }

        [Fact]
        public void Get_items_available_id_null()
        {
            using(var context = new DBContext(ContextOptions))
            {
                //ARRANGE
                var mySer = new DashboardAnalyticsService(context);
                string myid = null;
                
                //ACT
                void Act() => mySer.GetItemsAvailable(myid, "D");
                
                //ASSERT
                var exception = Assert.Throws<Exception>(Act);
                Assert.Equal("Inventory ID is null.", exception.Message);
            }
        }
        
        
        [Fact]
        public void Get_items_available_time_null()
        {
            using(var context = new DBContext(ContextOptions))
            {
                //ARRANGE
                var mySer = new DashboardAnalyticsService(context);
                string myid = "10000000-0000-0000-0000-000000000009";
                
                //ACT
                void Act() => mySer.GetItemsAvailable(myid, null);
                
                //ASSERT
                var exception = Assert.Throws<Exception>(Act);
                Assert.Equal("Time period is null", exception.Message);
            }
        }
        
        [Fact]
        public void Get_items_available_invalid_id()
        {
            using(var context = new DBContext(ContextOptions))
            {
                //ARRANGE
                var mySer = new DashboardAnalyticsService(context);
                string myid = "10000000-0000-0000-0000-000000000009";
                
                //ACT
                void Act() => mySer.GetItemsAvailable(myid, "D");
                
                //ASSERT
                var exception = Assert.Throws<Exception>(Act);
                Assert.Equal("Inventory with the given Inventory ID does not exist.", exception.Message);
            }
        }
        
        //Unit test defined to retrieve the items that are marked as sold
        [Fact]
        public void Get_Items_Sold()
        {
            using(var context = new DBContext(ContextOptions))
            {
                //ARRANGE
                var dashService = new DashboardAnalyticsService(context);

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
                
                var thenew = new Inventory {Id = theId2, name = "Integration test inventory", userId = u2};
                
                var item1 = new Item { Id = itemId1, name = "Leather jacket", inventoryId  = theId2, type = "Clothes", entryDate = DateTime.Now, quantity = 1, isSold = true};

                var item2 = new Item { Id = itemId2, name = "All star hoody", inventoryId  = theId2, type = "Clothes", entryDate = DateTime.Now, quantity = 2, isSold = true};

                var invSer = new InventoryService(context);
                var itemSer = new ItemService(context);
                
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
        }
        
        [Fact]
        public void Get_items_sold_id_null()
        {
            using(var context = new DBContext(ContextOptions))
            {
                //ARRANGE
                var mySer = new DashboardAnalyticsService(context);
                string myid = null;
                
                //ACT
                void Act() => mySer.GetItemsSold(myid, "D");
                
                //ASSERT
                var exception = Assert.Throws<Exception>(Act);
                Assert.Equal("Inventory ID is null.", exception.Message);
            }
        }
        
        
        [Fact]
        public void Get_items_sold_time_null()
        {
            using(var context = new DBContext(ContextOptions))
            {
                //ARRANGE
                var mySer = new DashboardAnalyticsService(context);
                string myid = "10000000-0000-0000-0000-000000000009";
                
                //ACT
                void Act() => mySer.GetItemsSold(myid, null);
                
                //ASSERT
                var exception = Assert.Throws<Exception>(Act);
                Assert.Equal("Time period is null", exception.Message);
            }
        }
        
        [Fact]
        public void Get_items_sold_invalid_id()
        {
            using(var context = new DBContext(ContextOptions))
            {
                //ARRANGE
                var mySer = new DashboardAnalyticsService(context);
                string myid = "10000000-0000-0000-0000-000000000009";
                
                //ACT
                void Act() => mySer.GetItemsSold(myid, "D");
                
                //ASSERT
                var exception = Assert.Throws<Exception>(Act);
                Assert.Equal("Inventory with the given Inventory ID does not exist.", exception.Message);
            }
        }
        

        //Unit test defined to calculate the revenue generated from sold items
        [Fact]
        public void Get_Revenue()
        {
            using(var context = new DBContext(ContextOptions))
            {
                //ARRANGE
                var dashService = new DashboardAnalyticsService(context);

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
                
                //-----------------------------------------

                var thenew = new Inventory {Id = theId2, name = "Integration test inventory", userId = u2};
                
                var item1 = new Item { Id = itemId1, name = "Cardigan", inventoryId  = theId2, type = "Clothes", entryDate = DateTime.Now
                    , quantity = 1, isSold = true, price = 100};

                var item2 = new Item { Id = itemId2, name = "Polo vest", inventoryId  = theId2, type = "Clothes", entryDate = DateTime.Now
                    , quantity = 2, isSold = true, price = 200};

                var invSer = new InventoryService(context);
                var itemSer = new ItemService(context);
                
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
        }

        //Unit test defined to calculate the growth of sales for a user and their inventories and items 
        [Fact]
        public void Get_Sales_Growth()
        {
            using(var context = new DBContext(ContextOptions))
            {
                //ARRANGE
                var dashService = new DashboardAnalyticsService(context);
                
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
                
                var thenew = new Inventory {Id = theId2, name = "Integration test inventory", userId = u2};
                
                var item1 = new Item { Id = itemId1, name = "Bootleg jeans", inventoryId  = theId2, type = "Clothes", entryDate = DateTime.Now};

                var item2 = new Item { Id = itemId2, name = "Turtle neck", inventoryId  = theId2, type = "Clothes", entryDate = DateTime.Now};

                var invSer = new InventoryService(context);
                var itemSer = new ItemService(context);
                
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
            
        }
        
        //Unit test defined to calculate the revenue statistics for the items 
        [Fact]
        public void Items_Revenue_Stat()
        {
            using(var context = new DBContext(ContextOptions))
            {
                //ARRANGE
                var dashService = new DashboardAnalyticsService(context);
     
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
                
                var thenew = new Inventory {Id = theId2, name = "Integration test inventory", userId = u2};
                
                var item1 = new Item { Id = itemId1, name = "Plain t-shirt", inventoryId  = theId2, type = "Clothes", entryDate = DateTime.Now};

                var item2 = new Item { Id = itemId2, name = "Sweatpants", inventoryId  = theId2, type = "Clothes", entryDate = DateTime.Now};

                var invSer = new InventoryService(context);
                var itemSer = new ItemService(context);
                
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
        }

        //Unit test defined to calculate the statistics of sold items 
        [Fact]
        public void Items_Sold_Stat()
        {
            using(var context = new DBContext(ContextOptions))
            {
                //ARRANGE
                var dashService = new DashboardAnalyticsService(context);
                
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
                
                var thenew = new Inventory {Id = theId2, name = "Integration test inventory", userId = u2};
                
                var item1 = new Item { Id = itemId1, name = "Leopard stripe shirt", inventoryId  = theId2, type = "Clothes", entryDate = DateTime.Now};

                var item2 = new Item { Id = itemId2, name = "Leopard stripe shirt", inventoryId  = theId2, type = "Clothes", entryDate = DateTime.Now};

                var invSer = new InventoryService(context);
                var itemSer = new ItemService(context);
                
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
        }
        
        //Unit test defined to retrieve the statistics of items that are available 
        [Fact]
        public void Item_Available_Stat()
        {
            using(var context = new DBContext(ContextOptions))
            {
                //ARRANGE
                var dashService = new DashboardAnalyticsService(context);
                
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
                
                var thenew = new Inventory {Id = theId2, name = "Integration test inventory", userId = u2};
                
                var item1 = new Item { Id = itemId1, name = "Leopard stripe shirt", inventoryId  = theId2, type = "Clothes", entryDate = DateTime.Now};

                var item2 = new Item { Id = itemId2, name = "Leopard stripe shirt", inventoryId  = theId2, type = "Clothes", entryDate = DateTime.Now};

                var invSer = new InventoryService(context);
                var itemSer = new ItemService(context);
                
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
        }
    }
}