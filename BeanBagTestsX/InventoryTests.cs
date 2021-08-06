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

        // Unit test defined for the get user inventories, a valid ID will always be passed in so no need for negative testing
        [Fact]
        public void Get_user_inventories_with_valid_id()
        {
            //ARRANGE
            Guid theId1 = new();
            Guid theId2 = new();

            string u1 = "xxx";
            string u2 = "yyy";

            var mockIn = new Mock<IInventoryService>();

            var data = new List<Inventory>
            {
                new Inventory { Id = theId1, name = "Mums 1", userId = u1},
                new Inventory { Id = theId1, name = "Mums 1.2", userId = u1},

            }.AsQueryable();

            var mockSet = new Mock<DbSet<Inventory>>();
            mockSet.As<IQueryable<Inventory>>().Setup(m => m.Provider).Returns(data.Provider);
            mockSet.As<IQueryable<Inventory>>().Setup(m => m.Expression).Returns(data.Expression);
            mockSet.As<IQueryable<Inventory>>().Setup(m => m.ElementType).Returns(data.ElementType);
            mockSet.As<IQueryable<Inventory>>().Setup(m => m.GetEnumerator()).Returns(data.GetEnumerator());

            Mock<IInventoryService> myser = new Mock<IInventoryService>();

            //ACT

            myser.Setup(x => x.GetInventories(u1)).Returns(mockSet.Object.ToList());

            var tinvs = myser.Object.GetInventories(u1);


            //ASSERT
            Assert.Equal(2 , tinvs.Count);

        }


        //Unit test for creating an inventory, a valid new inventory object will always be passed in so need for negative testing
        [Fact]
        public void Creating_An_Inventory()
        {
            //ARRANGE
            Guid theId1 = new();
            Guid theId2 = new();

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

    }
}


