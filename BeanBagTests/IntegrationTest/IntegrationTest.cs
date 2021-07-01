/*using System;
using System.Collections.Generic;
using Xunit;
using Moq;
using System.Linq;
using BeanBag.Controllers;
using BeanBag.Database;
using BeanBag.Models;
using Microsoft.EntityFrameworkCore;
namespace BeanBagUnitTesting.IntegrationTest
{
      // This class creates an integration test for the BeanBag system.
      public class IntegrationTest
      {

        //---------------------------------- POSITIVE TESTING----------------------------------------- 

        // Unit test for making sure valid items are added successfully to the inventory.
        [Fact]
        public void Test_Valid_Item_Is_Stored_Successfully()
        {
            // Arrange.
            var mockSet = new Mock<DbSet<ItemModel>>();
            var mockContext = new Mock<BeanBagContext>();
            mockContext.Setup(m => m.Items).Returns(mockSet.Object);

            // Act.
            var service = new Mock<ItemController>(mockContext.Object); 
            service.Object.AddItem("123", "chair",  "furniture", new DateTime() );

            // Assert.
            mockSet.Verify(m => m.Add(It.IsAny<ItemModel>()), Times.Once());
            mockContext.Verify(m => m.SaveChanges(), Times.Once());
        }
        
        // Unit test for testing valid items are added to the inventory and queryable as well.
        [Fact]
        public void Test_Valid_Items_Are_Queryable()
        {
            // Arrange.
            var data = new List<ItemModel>
            {
                new ItemModel { InventoryId = "123", ItemName = "Chair", ItemType = "Furniture", ScanDate = new DateTime()},
                new ItemModel { InventoryId = "345", ItemName = "Car", ItemType = "Vehicles", ScanDate = new DateTime()},

            }.AsQueryable();

            var mockSet = new Mock<DbSet<ItemModel>>();
            mockSet.As<IQueryable<ItemModel>>().Setup(m => m.Provider).Returns(data.Provider);
            mockSet.As<IQueryable<ItemModel>>().Setup(m => m.Expression).Returns(data.Expression);
            mockSet.As<IQueryable<ItemModel>>().Setup(m => m.ElementType).Returns(data.ElementType);
            mockSet.As<IQueryable<ItemModel>>().Setup(m => m.GetEnumerator()).Returns(data.GetEnumerator());
            
            var mockContext = new Mock<BeanBagContext>();
            mockContext.Setup(c => c.Items).Returns(mockSet.Object);
            
            // Act.
            var service = new Mock<ItemController>(mockContext.Object); 
            var items = service.Object.GetItems();

            // Assert.
            Assert.Equal(2,items.Count);
            Assert.Equal("123", items[1].InventoryId);
            Assert.Equal("Chair", items[1].ItemName);
            Assert.Equal("Furniture", items[1].ItemType);
            Assert.Equal("345", items[0].InventoryId);
            Assert.Equal("Car", items[0].ItemName);
            Assert.Equal("Vehicles", items[0].ItemType);
        }
        
         
        *//*---------------------------------- NEGATIVE TESTING----------------------------------------- *//*

        // Unit test for making sure invalid items are not added and not queryable in the inventory.
        [Fact]
        public void Test_Invalid_Items_Are_Not_Queryable()
        {
            // Arrange.
            var data = new List<ItemModel>
            {
                new ItemModel { InventoryId = "123", ItemName = "Chair", ItemType = "Furniture", ScanDate = new DateTime()},
                new ItemModel { InventoryId = "345", ItemName = "Car", ItemType = "Vehicles", ScanDate = new DateTime()},
                
            }.AsQueryable();

            var mockSet = new Mock<DbSet<ItemModel>>();
            mockSet.As<IQueryable<ItemModel>>().Setup(m => m.Provider).Returns(data.Provider);
            mockSet.As<IQueryable<ItemModel>>().Setup(m => m.Expression).Returns(data.Expression);
            mockSet.As<IQueryable<ItemModel>>().Setup(m => m.ElementType).Returns(data.ElementType);
            mockSet.As<IQueryable<ItemModel>>().Setup(m => m.GetEnumerator()).Returns(data.GetEnumerator());
            
            var mockContext = new Mock<BeanBagContext>();
            mockContext.Setup(c => c.Items).Returns(mockSet.Object);
            
            // Act.
            var service = new Mock<ItemController>(mockContext.Object); 
            var items = service.Object.GetItems();

            // Assert.
            Assert.Equal(2,items.Count);
            Assert.NotEqual("Invalid123", items[0].InventoryId);
            Assert.NotEqual("InvalidChair", items[0].ItemName);
            Assert.NotEqual("InvalidFurniture", items[0].ItemType);
            Assert.NotEqual("Invalid345", items[1].InventoryId);
            Assert.NotEqual("InvalidCar", items[1].ItemName);
            Assert.NotEqual("InvalidVehicle", items[1].ItemType);
        }
        
        // Unit test to ensure invalid items with invalid item details are not stored to the inventory.
        [Fact]
        public void Test_Invalid_Item_Not_Stored()
        {
            // Arrange.
            var mockSet = new Mock<DbSet<ItemModel>>();
            var mockContext = new Mock<BeanBagContext>();
            mockContext.Setup(m => m.Items).Returns(mockSet.Object);

            // Act.
            var service = new Mock<ItemController>(mockContext.Object); 
            
            // Add exception handling to Add Item implementation.
            service.Object.AddItem("wack-id", "chair",  "furniture", new DateTime() );
            
            // Assert.
            mockSet.Verify(m => m.Add(It.IsAny<ItemModel>()), Times.Once());
            mockContext.Verify(m => m.SaveChanges(), Times.Once()); 
        }
      }
}*/