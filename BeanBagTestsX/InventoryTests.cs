using System;
using System.Collections.Generic;
using System.Linq;
using BeanBag.Database;
using BeanBag.Models;
using BeanBag.Services;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;

namespace BeanBagUnitTests
{
    


    public class InventoryTests
    {

        // Unit test defined for the get user inventories, a valid ID will always be passed in so no need for negative testing
        [Fact]
        public void Get_user_inventories_with_valid_id()
        {
            //ARRANGE
            Guid theId1 = new("00000000-0000-0000-0000-000000000001");
            Guid theId2 = new("00000000-0000-0000-0000-000000000002");

            string u1 = "xxx";
            string u2 = "yyy";

            var mockIn = new Mock<IInventoryService>();

            var data = new List<Inventory>
            {
                new Inventory { Id = theId1, name = "Mums 1", userId = u1 },
                new Inventory { Id = theId2, name = "Mums 1.2", userId = u1 },
            }.AsQueryable();

            var mockSet = new Mock<DbSet<Inventory>>();
            mockSet.As<IQueryable<Inventory>>().Setup(m => m.Provider).Returns(data.Provider);
            mockSet.As<IQueryable<Inventory>>().Setup(m => m.Expression).Returns(data.Expression);
            mockSet.As<IQueryable<Inventory>>().Setup(m => m.ElementType).Returns(data.ElementType);
            mockSet.As<IQueryable<Inventory>>().Setup(m => m.GetEnumerator()).Returns(data.GetEnumerator());


            //ACT
            
            var dbMock = new Mock<DBContext>();
            dbMock.Setup(x => x.Set<Inventory>()).Returns(mockSet.Object);
            var myser = new InventoryService(dbMock.Object);
            
            var tinvs = myser.GetInventories(u1);
            
            //ASSERT
            Assert.Equal(2 , tinvs.Count);

        }


        //Unit test for creating an inventory, a valid new inventory object will always be passed in so need for negative testing
        [Fact]
        public void Creating_An_Inventory()
        {
            //ARRANGE
            Guid theId1 = new("00000000-0000-0000-0000-000000000001");
            Guid theId2 = new("00000000-0000-0000-0000-000000000002");

            string u1 = "xxx";
            string u2 = "yyy";

            var mockIn = new Mock<IInventoryService>();

            var data = new List<Inventory>
            {
                new Inventory { Id = theId1, name = "Mums 1", userId = u1 },
                new Inventory { Id = theId2, name = "Mums 1.2", userId = u1 },
            }.AsQueryable();

            var mockSet = new Mock<DbSet<Inventory>>();
            mockSet.As<IQueryable<Inventory>>().Setup(m => m.Provider).Returns(data.Provider);
            mockSet.As<IQueryable<Inventory>>().Setup(m => m.Expression).Returns(data.Expression);
            mockSet.As<IQueryable<Inventory>>().Setup(m => m.ElementType).Returns(data.ElementType);
            mockSet.As<IQueryable<Inventory>>().Setup(m => m.GetEnumerator()).Returns(data.GetEnumerator());


            //ACT
            
            var dbMock = new Mock<DBContext>();
            dbMock.Setup(x => x.Set<Inventory>()).Returns(mockSet.Object);
            var myser = new InventoryService(dbMock.Object);
            
            var tinvs = myser.FindInventory(theId1);
            
            //ASSERT
            Assert.Equal(theId2, tinvs.Id);
        }


        
        //Unit test for deleting an inventory. It will always be valid because a guid and user id will be passed in automatically 
        [Fact]
        public void Deleting_An_Inventory()
        {
            //ARRANGE
            Guid theId1 = new("00000000-0000-0000-0000-000000000001");
            Guid theId2 = new("00000000-0000-0000-0000-000000000002");

            string u1 = "xxx";
            string u2 = "yyy";

            var mockIn = new Mock<IInventoryService>();

            var data = new List<Inventory>
            {
                new Inventory { Id = theId1, name = "Mums 1", userId = u1 },
                new Inventory { Id = theId2, name = "Mums 1.2", userId = u1 },
            }.AsQueryable();

            var mockSet = new Mock<DbSet<Inventory>>();
            mockSet.As<IQueryable<Inventory>>().Setup(m => m.Provider).Returns(data.Provider);
            mockSet.As<IQueryable<Inventory>>().Setup(m => m.Expression).Returns(data.Expression);
            mockSet.As<IQueryable<Inventory>>().Setup(m => m.ElementType).Returns(data.ElementType);
            mockSet.As<IQueryable<Inventory>>().Setup(m => m.GetEnumerator()).Returns(data.GetEnumerator());


            //ACT
            
            var dbMock = new Mock<DBContext>();
            dbMock.Setup(x => x.Set<Inventory>()).Returns(mockSet.Object);
            var myser = new InventoryService(dbMock.Object);
            
            var isDeleted = myser.DeleteInventory(theId2, u1);
            var tinvs = myser.GetInventories(u1);
            
            //ASSERT
            Assert.True(isDeleted);
            Assert.Equal(1 , tinvs.Count);
        }
        
        
        [Fact]
        public void Editing_An_Inventory()
        {
            //ARRANGE
            Guid theId1 = new("00000000-0000-0000-0000-000000000001");
            Guid theId2 = new("00000000-0000-0000-0000-000000000002");

            string u1 = "xxx";
            string u2 = "yyy";

            var mockIn = new Mock<IInventoryService>();

            var data = new List<Inventory>
            {
                new Inventory { Id = theId1, name = "Mums 1", userId = u1 },
                new Inventory { Id = theId2, name = "Mums 1.2", userId = u1 },
            }.AsQueryable();

            var mockSet = new Mock<DbSet<Inventory>>();
            mockSet.As<IQueryable<Inventory>>().Setup(m => m.Provider).Returns(data.Provider);
            mockSet.As<IQueryable<Inventory>>().Setup(m => m.Expression).Returns(data.Expression);
            mockSet.As<IQueryable<Inventory>>().Setup(m => m.ElementType).Returns(data.ElementType);
            mockSet.As<IQueryable<Inventory>>().Setup(m => m.GetEnumerator()).Returns(data.GetEnumerator());


            //ACT
            
            var dbMock = new Mock<DBContext>();
            dbMock.Setup(x => x.Set<Inventory>()).Returns(mockSet.Object);
            var myser = new InventoryService(dbMock.Object);
            
            var isEdited = myser.EditInventory(u1, myser.FindInventory(theId2));

            //ASSERT
            Assert.True(isEdited);
        }
        
        
        [Fact]
        public void Find_An_Inventory_with_Id()
        {
            //ARRANGE
            Guid theId1 = new("00000000-0000-0000-0000-000000000001");
            Guid theId2 = new("00000000-0000-0000-0000-000000000002");

            string u1 = "xxx";
            string u2 = "yyy";

            var mockIn = new Mock<IInventoryService>();

            var data = new List<Inventory>
            {
                new Inventory { Id = theId1, name = "Mums 1", userId = u1 },
                new Inventory { Id = theId2, name = "Mums 1.2", userId = u1 },
            }.AsQueryable();

            var mockSet = new Mock<DbSet<Inventory>>();
            mockSet.As<IQueryable<Inventory>>().Setup(m => m.Provider).Returns(data.Provider);
            mockSet.As<IQueryable<Inventory>>().Setup(m => m.Expression).Returns(data.Expression);
            mockSet.As<IQueryable<Inventory>>().Setup(m => m.ElementType).Returns(data.ElementType);
            mockSet.As<IQueryable<Inventory>>().Setup(m => m.GetEnumerator()).Returns(data.GetEnumerator());


            //ACT
            
            var dbMock = new Mock<DBContext>();
            dbMock.Setup(x => x.Set<Inventory>()).Returns(mockSet.Object);
            var myser = new InventoryService(dbMock.Object);

            var foundInv = myser.FindInventory(theId2);

            //ASSERT
            Assert.Equal(theId2, foundInv.Id);
            Assert.NotNull(foundInv);
        }

    }

    


}


