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
        public void QueryInventoryFromSQLTest()
        {
            Guid theId2 = new Guid();
            Guid theId3 = new Guid();
            Guid theId4 = new Guid();
            string u2 = "9999";
            string u3 = "9998";
            
            DateTime myDay = DateTime.MinValue;

            //Add some monsters before querying
            _context.Inventories.Add(new Inventory { Id = theId2, name = "Leopard shorts", createdDate = myDay, userId = u2 });
            _context.Inventories.Add(new Inventory { Id = theId3, name = "Zebra shirt",  createdDate = myDay, userId = u2 });
            _context.Inventories.Add(new Inventory { Id = theId4, name = "Kudu sandals" , createdDate = myDay, userId = u3 });
           
            _context.SaveChanges();

            //Execute the query
            InventoryService query = new InventoryService(_context);
            var getInvs = query.GetInventories(u2);

            //Verify the results
            Assert.Equal(2, getInvs.Count);
           // var scaryMonster = scaryMonsters.First();
           // Assert.Equal("Imposter Monster", scaryMonster.Name);
           // Assert.Equal("Red", scaryMonster.Colour);
           // Assert.True(scaryMonster.IsScary);
        }

    }
}
