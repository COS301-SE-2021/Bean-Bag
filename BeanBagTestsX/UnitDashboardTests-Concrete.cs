using System;
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

                var m = dashService.GetRecentItems(theId2.ToString());

                //ASSERT
                Assert.NotNull(m);

                itemSer.DeleteItem(itemId1);
                itemSer.DeleteItem(itemId2);
                invSer.DeleteInventory(theId2, u2);
            }
        }

        //Unit test defined for getting the total amount of items across all of a person's inventories
        [Fact]
        public void Get_Total_Items()
        {
            //ARRANGE
            
            
            //ACT
            
            
            //ASSERT
            
        }
        
        //Unit test defined to get the top items from a person's inventories based off a quantifier (i.e. price)
        [Fact]
        public void Get_Top_Items()
        {
            //ARRANGE
            
            
            //ACT
            
            
            //ASSERT
        }
    
        //Unit test defined to get the items available, currently, in a user's database
        [Fact]
        public void Get_Items_Available()
        {
            //ARRANGE
            
            
            //ACT
            
            
            //ASSERT
            
        }

        //Unit test defined to retrieve the items that are marked as sold
        [Fact]
        public void Get_Items_Sold()
        {
            //ARRANGE
            
            
            //ACT
            
            
            //ASSERT
            
        }

        //Unit test defined to calculate the revenue generated from sold items
        [Fact]
        public void Get_Revenue()
        {
            //ARRANGE
            
            
            //ACT
            
            
            //ASSERT
            
        }

        //Unit test defined to calculate the growth of sales for a user and their inventories and items 
        [Fact]
        public void Get_Sales_Growth()
        {
            //ARRANGE
            
            
            //ACT
            
            
            //ASSERT
            
        }
        
        //Unit test defined to calculate the revenue statistics for the items 
        [Fact]
        public void Items_Revenue_Stat()
        {
            //ARRANGE
            
            
            //ACT
            
            
            //ASSERT
            
        }

        //Unit test defined to calculate the statistics of sold items 
        [Fact]
        public void Items_Sold_Stat()
        {
            //ARRANGE
            
            
            //ACT
            
            
            //ASSERT
            
        }
        
        //Unit test defined to retrieve the statistics of items that are available 
        [Fact]
        public void Item_Available_Stat()
        {
            //ARRANGE
            
            
            //ACT
            
            
            //ASSERT
            
        }
    }
}