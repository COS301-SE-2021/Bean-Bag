using BeanBag;
using BeanBag.Database;
using BeanBag.Models;
using BeanBag.Services;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace BeanBagIntegrationTests
{
    public class IntegrationDBtest 
    {

        DBContext _context;

        public IntegrationDBtest()
        {
            var serviceProvider = new ServiceCollection()
                .AddEntityFrameworkSqlServer()
                .BuildServiceProvider();

            var builder = new DbContextOptionsBuilder<DBContext>();

            builder.UseSqlServer($"Server=tcp:polariscapestone.database.windows.net,1433;Initial Catalog=Bean-Bag-Platform-DB;Persist Security Info=False;User ID=polaris; Password=MNRSSp103;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;")
                    .UseInternalServiceProvider(serviceProvider);

            _context = new DBContext(builder.Options);

        }

        [Fact]
        public void Query_Inventory_From_SQL_Test()
        {
            //ARRANGE
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
            
            Guid theId2 = new("00000000-0000-0000-0000-0000000" + myGuidEnd);

            var theId3 = new Guid();
            var theId4 = new Guid();
            
            var myDay = DateTime.MinValue;
            
            //ACT
            _context.Inventories.Add(new Inventory { Id = theId2, name = "Leopard shorts", createdDate = myDay, userId = u2 });
            _context.Inventories.Add(new Inventory { Id = theId3, name = "Zebra shirt",  createdDate = myDay, userId = u2 });
           
            _context.SaveChanges();

            InventoryService query = new InventoryService(_context);
            var getInvs = query.GetInventories(u2);

            //ASSERT
            Assert.Equal(2, getInvs.Count);
            var toCheck = _context.Inventories.Find(theId2);
            
            
           Assert.Equal("Leopard shorts", toCheck.name);
           Assert.Equal(u2, toCheck.userId);
        }
        
        
        
        
        
        [Fact]
        public void Create_Inventory_From_SQL()
        {
            //ARRANGE
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
            
            Guid theId2 = new("00000000-0000-0000-0000-0000000" + myGuidEnd);

            var theId3 = new Guid();
            var theId4 = new Guid();
            
            var myDay = DateTime.MinValue;

            //ACT
            var query = new InventoryService(_context);

            var thenew = new Inventory { Id = theId2, name = "Integration test inventory", userId = u2 };

            query.CreateInventory(thenew);
            
            var getInvs = query.FindInventory(theId2);

            //ASSERT
            Assert.NotNull(getInvs);
        }
        
        [Fact]
        public void Edit_Inventory_From_SQL()
        {
            //ARRANGE
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
            
            Guid theId2 = new("00000000-0000-0000-0000-0000000" + myGuidEnd);

            var theId3 = new Guid();
            var theId4 = new Guid();

            var myDay = DateTime.MinValue;
            
            //ACT
            
            var query = new InventoryService(_context);

            var thenew = new Inventory { Id = theId2, name = "Integration test inventory", userId = u2 };

            query.CreateInventory(thenew);

            var isUpdated = query.EditInventory(u2, thenew);
            
            var getInvs = query.FindInventory(theId2);

            //ASSERT
            Assert.True(isUpdated);
        }
        
        [Fact]
        public void Delete_Inventory_From_SQL()
        {
            //ARRANGE
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
            
            Guid theId2 = new("00000000-0000-0000-0000-0000000" + myGuidEnd);

            var theId3 = new Guid();
            var theId4 = new Guid();
            
            DateTime myDay = DateTime.MinValue;
            
            var query = new InventoryService(_context);
            
            var thenew = new Inventory { Id = theId2, name = "Integration test inventory", userId = u2 };

            //ACT 
            
            query.CreateInventory(thenew);

            var isDeleted = query.DeleteInventory(thenew.Id , u2);
            
            var getInvs = query.FindInventory(theId2);

            //ASSERT
            Assert.True(isDeleted);
            Assert.Null(getInvs);
        }
        
        
        [Fact]
        public void Edit_Item()
        {
            //ARRANGE
            var chars = "0123456789";
            var stringChars = new char[5];
            var random = new Random();

            for (int i = 0; i < stringChars.Length; i++)
            {
                stringChars[i] = chars[random.Next(chars.Length)];
            }

            var finalString = new String(stringChars);

            var myGuidEnd = finalString;

            string u2 = finalString.Substring(0, 4);
            string u3 = finalString.Substring(1, 4);
            
            Guid theId2 = new("00000000-0000-0000-0000-0000000" + myGuidEnd);
            Guid itemId = new("00000000-0000-0000-0000-0000000" + u3 + "3");

            DateTime myDay = DateTime.MinValue;
            

            //Execute the query
            var query = new ItemService(_context);

            var invSer = new InventoryService(_context);
            
            Inventory myinv = new Inventory { Id = theId2, name = "Integration test inventory", userId = u2 };
            var thenew = new Item { Id = itemId, name = "Leopard stripe shirt", inventoryId  = theId2, soldDate = DateTime.Now, type = "Clothes"};

            //ACT 
            invSer.CreateInventory(myinv);
            query.CreateItem(thenew);
            query.EditItem(thenew);

            

            
            
            //ASSERT
            
            Assert.Equal(thenew.soldDate, DateTime.MinValue);
            query.DeleteItem(thenew.Id);
        }
        
        
        [Fact]
        public void Delete_Item()
        {
            //ARRANGE
            var chars = "0123456789";
            var stringChars = new char[5];
            var random = new Random();

            for (int i = 0; i < stringChars.Length; i++)
            {
                stringChars[i] = chars[random.Next(chars.Length)];
            }

            var finalString = new String(stringChars);

            var myGuidEnd = finalString;

            string u2 = finalString.Substring(0, 4);
            string u3 = finalString.Substring(1, 4);
            
            Guid theId2 = new("00000000-0000-0000-0000-0000000" + myGuidEnd);
            Guid itemId = new("00000000-0000-0000-0000-0000000" + u3 + "3");
            Guid itemId2 = new("00000000-0000-0000-0000-0000000" + u3 + "0");

            DateTime myDay = DateTime.MinValue;
            

            //Execute the query
            var query = new ItemService(_context);

            var invSer = new InventoryService(_context);
            
            Inventory myinv = new Inventory { Id = theId2, name = "Integration test inventory", userId = u2 };
            var thenew = new Item { Id = itemId, name = "Leopard stripe shirt", inventoryId  = theId2, soldDate = DateTime.Now, type = "Clothes"};
            var thenew2 = new Item { Id = itemId2, name = "Leopard stripe shirt", inventoryId  = theId2, soldDate = DateTime.Now, type = "Clothes"};

            //ACT 
            invSer.CreateInventory(myinv);
            query.CreateItem(thenew);
            query.CreateItem(thenew2);
            var isDel = query.DeleteItem(itemId2);

            var mycheck = query.GetItems(theId2);
            var myCount = mycheck.Count;
            //ASSERT
            
            Assert.True(isDel);
            Assert.Equal(1, myCount);
            query.DeleteItem(thenew.Id);
            query.DeleteItem(thenew2.Id);
            invSer.DeleteInventory(theId2, u2);
        }
        
        [Fact]
        public void Get_InvId_From_Item()
        {
            //ARRANGE
            var chars = "0123456789";
            var stringChars = new char[5];
            var random = new Random();

            for (int i = 0; i < stringChars.Length; i++)
            {
                stringChars[i] = chars[random.Next(chars.Length)];
            }

            var finalString = new String(stringChars);

            var myGuidEnd = finalString;

            string u2 = finalString.Substring(0, 4);
            string u3 = finalString.Substring(1, 4);
            
            Guid theId2 = new("00000000-0000-0000-0000-0000000" + myGuidEnd);
            Guid itemId = new("00000000-0000-0000-0000-0000000" + u3 + "3");
            Guid itemId2 = new("00000000-0000-0000-0000-0000000" + u3 + "0");

            DateTime myDay = DateTime.MinValue;
            

            //Execute the query
            var query = new ItemService(_context);

            var invSer = new InventoryService(_context);
            
            Inventory myinv = new Inventory { Id = theId2, name = "Integration test inventory", userId = u2 };
            var thenew = new Item { Id = itemId, name = "Leopard stripe shirt", inventoryId  = theId2, soldDate = DateTime.Now, type = "Clothes"};
            var thenew2 = new Item { Id = itemId2, name = "Leopard stripe shirt", inventoryId  = theId2, soldDate = DateTime.Now, type = "Clothes"};

            //ACT 
            invSer.CreateInventory(myinv);
            query.CreateItem(thenew);
            query.CreateItem(thenew2);
            var isFound = query.GetInventoryIdFromItem(thenew2.Id);
            
            //ASSERT
            
            Assert.Equal(myinv.Id, isFound);
            query.DeleteItem(thenew.Id);
            query.DeleteItem(thenew2.Id);
            invSer.DeleteInventory(theId2, u2);
        }
            
    }
}
