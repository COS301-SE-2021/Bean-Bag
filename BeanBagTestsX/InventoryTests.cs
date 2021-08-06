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
        //private readonly InventoryService _iut;
        //private readonly Mock<DBContext> _invmock = new Mock<DBContext>();


        //public InventoryTests()
        //{
        //    _iut = new InventoryService(_invmock.Object);

        //}

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

            //var mockContext = new Mock<DBContext>();
            //mockContext.Setup(c => c.Inventories).Returns(mockSet.Object);

            //ACT

            //var service = new InventoryService(mockContext.Object);
            Mock<IInventoryService> myser = new Mock<IInventoryService>();

            myser.Setup(x => x.GetInventories(u1)).Returns(mockSet.Object.ToList());


            //var service = _iut;
            //var invs = service.GetInventories(u1);
            var tinvs = myser.Object.GetInventories(u1);


            //ASSERT
            Assert.Equal(2 , tinvs.Count);

        }

    }
}


