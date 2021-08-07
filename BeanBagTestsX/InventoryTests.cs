using System;
using System.Collections.Generic;
using System.Linq;
using BeanBag.Database;
using BeanBag.Models;
using BeanBag.Services;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;

namespace BeanBagTestsX
{
    
    
    public class InventoryTests
    {
        
         
        //Unit test for creating an inventory, a valid new inventory object will always be passed in so need for negative testing
        [Fact]
        public void Creating_An_Inventory()
        {
            //ARRANGE
            Guid theId2 = new("00000000-0000-0000-0000-000000000001");

            string u1 = "xxx";
            string u2 = "yyy";

            var mockIn = new Mock<IInventoryService>();

            var data = new List<Inventory>
            {

            }.AsQueryable();

            var mockSet = new Mock<DbSet<Inventory>>();
            mockSet.As<IQueryable<Inventory>>().Setup(m => m.Provider).Returns(data.Provider);
            mockSet.As<IQueryable<Inventory>>().Setup(m => m.Expression).Returns(data.Expression);
            mockSet.As<IQueryable<Inventory>>().Setup(m => m.ElementType).Returns(data.ElementType);
            mockSet.As<IQueryable<Inventory>>().Setup(m => m.GetEnumerator()).Returns(data.GetEnumerator());

            //ACT

            Mock<IInventoryService> myser = new Mock<IInventoryService>();
            myser.Setup(x => x.GetInventories(u1)).Returns(mockSet.Object.ToList());

            Inventory thenew = new Inventory { Id = theId2, name = "Mums 2", userId = u2 };
            myser.Object.CreateInventory(thenew);


            //ASSERT
            Assert.NotNull(data);
        }


        //Unit test for deleting an inventory. It will always be valid because a guid and user id will be passed in automatically 
        [Fact]
        public void Deleting_An_Inventory()
        {
            //ARRANGE
            Guid theId1 = new("00000000-0000-0000-0000-000000000001");
            Guid theId2 = new("00000000-0000-0000-0000-000000000002");


            string u1 = "123";


            //var mockIn = new Mock<IInventoryService>();

            var data = new List<Inventory>
            {
                new Inventory { Id = theId1, name = "testinv 1", userId = u1},

            }.AsQueryable();

            var mockSet = new Mock<DbSet<Inventory>>();
            mockSet.As<IQueryable<Inventory>>().Setup(m => m.Provider).Returns(data.Provider);
            mockSet.As<IQueryable<Inventory>>().Setup(m => m.Expression).Returns(data.Expression);
            mockSet.As<IQueryable<Inventory>>().Setup(m => m.ElementType).Returns(data.ElementType);
            mockSet.As<IQueryable<Inventory>>().Setup(m => m.GetEnumerator()).Returns(data.GetEnumerator());

            //ACT

            Mock<IInventoryService> myser = new Mock<IInventoryService>();

            myser.Setup(x => x.GetInventories(u1)).Returns(mockSet.Object.ToList());


            bool b = myser.Object.DeleteInventory(theId1, u1);

            var updInvs = myser.Object.GetInventories(u1);
            int x = updInvs.Count;

            //ASSERT
            Assert.True(!b);
        }
        
        
        //Unit test for testing the edit inventory method of the inventory service class
        [Fact]
        public void Editing_An_Inventory()
        {
            //ARRANGE
            Guid theId1 = new("00000000-0000-0000-0000-000000000001");
            string u1 = "123";


            //var mockIn = new Mock<IInventoryService>();

            var data = new List<Inventory>
            {
                new Inventory { Id = theId1, name = "testinv 1", userId = u1},

            }.AsQueryable();

            var mockSet = new Mock<DbSet<Inventory>>();
            mockSet.As<IQueryable<Inventory>>().Setup(m => m.Provider).Returns(data.Provider);
            mockSet.As<IQueryable<Inventory>>().Setup(m => m.Expression).Returns(data.Expression);
            mockSet.As<IQueryable<Inventory>>().Setup(m => m.ElementType).Returns(data.ElementType);
            mockSet.As<IQueryable<Inventory>>().Setup(m => m.GetEnumerator()).Returns(data.GetEnumerator());

            //ACT

            Mock<IInventoryService> myser = new Mock<IInventoryService>();

            myser.Setup(x => x.GetInventories(u1)).Returns(mockSet.Object.ToList());


            bool b = myser.Object.EditInventory(u1, myser.Object.FindInventory(theId1));

            var updInvs = myser.Object.GetInventories(u1);
            int x = updInvs.Count;

            //ASSERT
            Assert.True(!b);
        }
        
        
        [Fact]
        public void Finding_An_Inventory()
        {
            //ARRANGE
            Guid theId1 = new("00000000-0000-0000-0000-000000000001");


            string u1 = "123";


            //var mockIn = new Mock<IInventoryService>();

            var data = new List<Inventory>
            {
                new Inventory { Id = theId1, name = "testinv 1", userId = u1},

            }.AsQueryable();

            var mockSet = new Mock<DbSet<Inventory>>();
            mockSet.As<IQueryable<Inventory>>().Setup(m => m.Provider).Returns(data.Provider);
            mockSet.As<IQueryable<Inventory>>().Setup(m => m.Expression).Returns(data.Expression);
            mockSet.As<IQueryable<Inventory>>().Setup(m => m.ElementType).Returns(data.ElementType);
            mockSet.As<IQueryable<Inventory>>().Setup(m => m.GetEnumerator()).Returns(data.GetEnumerator());

            //ACT

            Mock<IInventoryService> myser = new Mock<IInventoryService>();

            myser.Setup(x => x.GetInventories(u1)).Returns(mockSet.Object.ToList());


            Inventory b = myser.Object.FindInventory(theId1);
            

            //ASSERT
            Assert.Null(b);
        }


    }
}


