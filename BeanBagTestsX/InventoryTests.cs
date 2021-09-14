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

        // Unit test defined for the get user inventories (positive testing)
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
        
        
        // Unit test defined for the get user inventories (negative testing)
        [Fact]
        public void Get_user_inventories_with_invalid_id()
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
            Assert.NotEqual(3 , tinvs.Count);

        }


        private readonly DbContextOptions<DBContext> _options;
        
        //Unit test for creating an inventory, a valid new inventory (positive testing)
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
        
        //Unit test defined to test the inventory create, expecting failure (negative testing)
        [Fact]
        public void Creating_An_Inventory_invalid()  
        {
            //ARRANGE
            Guid theId1 = new("00000000-0000-0000-0000-000000000001");
            Guid theId2 = new("00000000-0000-0000-0000-000000000002");

            string u1 = "xxx";
            string u2 = "yyy";

            var mockIn = new Mock<IInventoryService>();

            var data = new List<Inventory>
            {
                new Inventory { Id = theId1, name = "Mums 1", userId = u1 }
                
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

            myser.CreateInventory(new Inventory {Id = theId2, name = "Mums 1.2", userId = u1});
            
            var tinvs = myser.FindInventory(theId2);
            
            //ASSERT
            Assert.NotEqual(theId1, tinvs.Id);
        }
        


        
        //Unit test for deleting an inventory (positive testing)
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
        
        //Unit test for deleting an inventory (negative testing)
        [Fact]
        public void Deleting_An_Inventory_invalid()
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
            
            var isDeleted = myser.DeleteInventory(theId2, u2);
            var tinvs = myser.GetInventories(u1);
            
            //ASSERT
            Assert.False(isDeleted);
            Assert.NotEqual(3 , tinvs.Count);
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
        
        //Unit testing for editing and inventory expecting failure (negative testing)
        [Fact]
        public void Editing_An_Inventory_invalid()
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
            
            var isEdited = myser.EditInventory(u2, myser.FindInventory(theId2));

            //ASSERT
            Assert.False(isEdited);
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
        
        //Unit test for find inv with ID, expecting failure (negative testing)
        [Fact]
        public void Find_An_Inventory_with_Id_invalid()
        {
            //ARRANGE
            Guid theId1 = new("00000000-0000-0000-0000-000000000001");
            Guid theId2 = new("00000000-0000-0000-0000-000000000002");
            Guid theId3 = new("00000000-0000-0000-0000-000000000003");

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

            var foundInv = myser.FindInventory(theId3);

            //ASSERT
            Assert.Null(foundInv);
        }
        

    }

    


}


