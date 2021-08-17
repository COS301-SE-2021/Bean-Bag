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
            
            Guid theId2 = new("00000000-0000-0000-0000-0000000" + myGuidEnd);

            Guid theId3 = new Guid();
            Guid theId4 = new Guid();
           

            
            DateTime myDay = DateTime.MinValue;
            


            //Add some monsters before querying
            _context.Inventories.Add(new Inventory { Id = theId2, name = "Leopard shorts", createdDate = myDay, userId = u2 });
            _context.Inventories.Add(new Inventory { Id = theId3, name = "Zebra shirt",  createdDate = myDay, userId = u2 });
            //_context.Inventories.Add(new Inventory { Id = theId4, name = "Kudu sandals" , createdDate = myDay, userId = u3 });
           
            _context.SaveChanges();

            //Execute the query
            InventoryService query = new InventoryService(_context);
            var getInvs = query.GetInventories(u2);

            //Verify the results
            Assert.Equal(2, getInvs.Count);
            var toCheck = _context.Inventories.Find(theId2);
            
            
           Assert.Equal("Leopard shorts", toCheck.name);
           Assert.Equal(u2, toCheck.userId);
           // Assert.True(scaryMonster.IsScary);
        }
        
        
        
        
        
        [Fact]
        public void Create_Inventory_From_SQL()
        {
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
            
            Guid theId2 = new("00000000-0000-0000-0000-0000000" + myGuidEnd);

            Guid theId3 = new Guid();
            Guid theId4 = new Guid();
           

            
            DateTime myDay = DateTime.MinValue;
            

            //Execute the query
            InventoryService query = new InventoryService(_context);
            
            
            
            Inventory thenew = new Inventory { Id = theId2, name = "Integration test inventory", userId = u2 };

            query.CreateInventory(thenew);
            
            var getInvs = query.FindInventory(theId2);

            //Verify the results
            Assert.NotNull(getInvs);
            // Assert.True(scaryMonster.IsScary);
        }
        
        [Fact]
        public void Edit_Inventory_From_SQL()
        {
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
            
            Guid theId2 = new("00000000-0000-0000-0000-0000000" + myGuidEnd);

            Guid theId3 = new Guid();
            Guid theId4 = new Guid();
           

            
            DateTime myDay = DateTime.MinValue;
            

            //Execute the query
            InventoryService query = new InventoryService(_context);
            
            
            
            Inventory thenew = new Inventory { Id = theId2, name = "Integration test inventory", userId = u2 };

            query.CreateInventory(thenew);

            var isUpdated = query.EditInventory(u2, thenew);
            
            var getInvs = query.FindInventory(theId2);

            //Verify the results
            Assert.True(isUpdated);
            // Assert.True(scaryMonster.IsScary);
        }
        
        [Fact]
        public void Delete_Inventory_From_SQL()
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
            
            Guid theId2 = new("00000000-0000-0000-0000-0000000" + myGuidEnd);

            Guid theId3 = new Guid();
            Guid theId4 = new Guid();
           

            
            DateTime myDay = DateTime.MinValue;
            

            //Execute the query
            InventoryService query = new InventoryService(_context);
            
            
            
            Inventory thenew = new Inventory { Id = theId2, name = "Integration test inventory", userId = u2 };

            //ACT 
            query.CreateInventory(thenew);

            var isDeleted = query.DeleteInventory(thenew.Id , u2);
            
            var getInvs = query.FindInventory(theId2);

            
            
            //ASSERT
            //Verify the results
            Assert.True(isDeleted);
            Assert.Null(getInvs);
            // Assert.True(scaryMonster.IsScary);
        }

    }
}
