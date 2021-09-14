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



    public class ItemTests
    {

        // Unit test defined for adding a QR code to an item using the azure function
        [Fact]
        public void Add_QRitem_with_valid_item()
        {
            //ARRANGE
            var testItem = new Item();
            var mockIn = new Mock<IItemService>();
            
            //ACT
            mockIn.Object.AddQrItem(testItem);

            //ASSERT
            Assert.NotEqual("", testItem.QRCodeLink);

        }

        //Unit test defined for creating an item
        [Fact]
        public void Create_item()
        {
            //ARRANGE

            Guid invId = new("10000000-0000-0000-0000-000000000001");
            Guid itemId = new("10000000-0000-0000-0000-000000000002");

            var myInv = new Inventory {Id = invId, name = "testInv", userId = "123"};
                
            var data = new List<Item>
            {

            }.AsQueryable();
            
            var mockSet = new Mock<DbSet<Item>>();
            mockSet.As<IQueryable<Item>>().Setup(m => m.Provider).Returns(data.Provider);
            mockSet.As<IQueryable<Item>>().Setup(m => m.Expression).Returns(data.Expression);
            mockSet.As<IQueryable<Item>>().Setup(m => m.ElementType).Returns(data.ElementType);
            mockSet.As<IQueryable<Item>>().Setup(m => m.GetEnumerator()).Returns(data.GetEnumerator());

            var mockIn = new Mock<IItemService>();
            var testItem = new Item();
            testItem.inventoryId = invId;
            

            //ACT
            
            var dbMock = new Mock<DBContext>();
            dbMock.Setup(x => x.Set<Item>()).Returns(mockSet.Object);
            var myser = new ItemService(dbMock.Object);
            
            var thenew = new Item { Id = itemId, name = "Leopard stripe shirt", inventoryId  = invId, soldDate = DateTime.Now, type = "Clothes"};

            myser.CreateItem(thenew);

            //ASSERT
            
            Assert.Equal(itemId, thenew.Id);
            

        }

        //Unit test defined for deleting an item 
        [Fact]
        public void Delete_item()
        {
            //ARRANGE

            Guid invId = new("10000000-0000-0000-0000-000000000001");
            Guid itemId = new("10000000-0000-0000-0000-000000000002");
            Guid itemId2 = new("10000000-0000-0000-0000-000000000003");

            var myInv = new Inventory {Id = invId, name = "testInv", userId = "123"};
                
            var data = new List<Item>
            {

            }.AsQueryable();
            
            var mockSet = new Mock<DbSet<Item>>();
            mockSet.As<IQueryable<Item>>().Setup(m => m.Provider).Returns(data.Provider);
            mockSet.As<IQueryable<Item>>().Setup(m => m.Expression).Returns(data.Expression);
            mockSet.As<IQueryable<Item>>().Setup(m => m.ElementType).Returns(data.ElementType);
            mockSet.As<IQueryable<Item>>().Setup(m => m.GetEnumerator()).Returns(data.GetEnumerator());

            var mockIn = new Mock<IItemService>();
            var testItem = new Item();
            testItem.inventoryId = invId;
            

            //ACT
            
            var dbMock = new Mock<DBContext>();
            dbMock.Setup(x => x.Set<Item>()).Returns(mockSet.Object);
            var myser = new ItemService(dbMock.Object);
            
            var thenew = new Item { Id = itemId, name = "Leopard stripe shirt", inventoryId  = invId, soldDate = DateTime.Now, type = "Clothes"};
            var thenew2 = new Item { Id = itemId2, name = "Grey sandals", inventoryId  = invId, soldDate = DateTime.Now, type = "Clothes"};

            myser.CreateItem(thenew);
            myser.CreateItem(thenew2);

            var isDel = myser.DeleteItem(itemId);
            var itemCount = myser.GetItems(invId);
            var counts = itemCount.Count;
            
            //ASSERT
            
            Assert.Equal(itemId, thenew.Id);

        }
        
        
        
        
    //Unit test defined for deleting an item (negative testing)
            [Fact]
            public void Delete_item_invalid()
            {
                //ARRANGE
    
                Guid invId = new("10000000-0000-0000-0000-000000000001");
                Guid itemId = new("10000000-0000-0000-0000-000000000002");
                Guid itemId2 = new("10000000-0000-0000-0000-000000000003");
    
                var myInv = new Inventory {Id = invId, name = "testInv", userId = "123"};
                    
                var data = new List<Item>
                {
    
                }.AsQueryable();
                
                var mockSet = new Mock<DbSet<Item>>();
                mockSet.As<IQueryable<Item>>().Setup(m => m.Provider).Returns(data.Provider);
                mockSet.As<IQueryable<Item>>().Setup(m => m.Expression).Returns(data.Expression);
                mockSet.As<IQueryable<Item>>().Setup(m => m.ElementType).Returns(data.ElementType);
                mockSet.As<IQueryable<Item>>().Setup(m => m.GetEnumerator()).Returns(data.GetEnumerator());
    
                var mockIn = new Mock<IItemService>();
                var testItem = new Item();
                testItem.inventoryId = invId;
                
    
                //ACT
                
                var dbMock = new Mock<DBContext>();
                dbMock.Setup(x => x.Set<Item>()).Returns(mockSet.Object);
                var myser = new ItemService(dbMock.Object);
                
                var thenew = new Item { Id = itemId, name = "Leopard stripe shirt", inventoryId  = invId, soldDate = DateTime.Now, type = "Clothes"};
    
                myser.CreateItem(thenew);
    
                var isDel = myser.DeleteItem(itemId2);
                var itemCount = myser.GetItems(invId);
                var counts = itemCount.Count;
                
                //ASSERT
                Assert.False(isDel);
                Assert.Equal(1, counts);
    
            }

        //Unit test defined for editing an item (negative testing)
        [Fact]
        public void Edit_item()
        {
            //ARRANGE

            Guid invId = new("10000000-0000-0000-0000-000000000001");
            Guid itemId = new("10000000-0000-0000-0000-000000000002");
            Guid itemId2 = new("10000000-0000-0000-0000-000000000003");

            var myInv = new Inventory {Id = invId, name = "testInv", userId = "123"};
                
            var data = new List<Item>
            {

            }.AsQueryable();
            
            var mockSet = new Mock<DbSet<Item>>();
            mockSet.As<IQueryable<Item>>().Setup(m => m.Provider).Returns(data.Provider);
            mockSet.As<IQueryable<Item>>().Setup(m => m.Expression).Returns(data.Expression);
            mockSet.As<IQueryable<Item>>().Setup(m => m.ElementType).Returns(data.ElementType);
            mockSet.As<IQueryable<Item>>().Setup(m => m.GetEnumerator()).Returns(data.GetEnumerator());

            var mockIn = new Mock<IItemService>();
            var testItem = new Item();
            testItem.inventoryId = invId;
            

            //ACT
            
            var dbMock = new Mock<DBContext>();
            dbMock.Setup(x => x.Set<Item>()).Returns(mockSet.Object);
            var myser = new ItemService(dbMock.Object);
            
            var thenew = new Item { Id = itemId, name = "Leopard stripe shirt", inventoryId  = invId, soldDate = DateTime.Now, type = "Clothes"};
            var thenew2 = new Item { Id = itemId2, name = "Grey sandals", inventoryId  = invId, soldDate = DateTime.Now, type = "Clothes", isSold = true};

            myser.CreateItem(thenew);
            myser.CreateItem(thenew2);

            myser.EditItem(myser.FindItem(itemId2));

            //ASSERT
            
            Assert.NotEqual(DateTime.MinValue, myser.FindItem(itemId2).soldDate);

        }

        //Unit test defined to find an item from the database
        [Fact]
        public void Find_item()
        {
            //ARRANGE

            Guid invId = new("10000000-0000-0000-0000-000000000001");
            Guid itemId = new("10000000-0000-0000-0000-000000000002");
            Guid itemId2 = new("10000000-0000-0000-0000-000000000003");

            var myInv = new Inventory {Id = invId, name = "testInv", userId = "123"};
                
            var data = new List<Item>
            {

            }.AsQueryable();
            
            var mockSet = new Mock<DbSet<Item>>();
            mockSet.As<IQueryable<Item>>().Setup(m => m.Provider).Returns(data.Provider);
            mockSet.As<IQueryable<Item>>().Setup(m => m.Expression).Returns(data.Expression);
            mockSet.As<IQueryable<Item>>().Setup(m => m.ElementType).Returns(data.ElementType);
            mockSet.As<IQueryable<Item>>().Setup(m => m.GetEnumerator()).Returns(data.GetEnumerator());

            var mockIn = new Mock<IItemService>();
            var testItem = new Item();
            testItem.inventoryId = invId;
            

            //ACT
            
            var dbMock = new Mock<DBContext>();
            dbMock.Setup(x => x.Set<Item>()).Returns(mockSet.Object);
            var myser = new ItemService(dbMock.Object);
            
            var thenew = new Item { Id = itemId, name = "Leopard stripe shirt", inventoryId  = invId, soldDate = DateTime.Now, type = "Clothes"};
            var thenew2 = new Item { Id = itemId2, name = "Grey sandals", inventoryId  = invId, soldDate = DateTime.Now, type = "Clothes"};

            myser.CreateItem(thenew);
            myser.CreateItem(thenew2);

            var isFound = myser.FindItem(itemId);

            //ASSERT
            
            Assert.Equal(itemId, isFound.Id);
            
        }

        //Unit test defined to retrieve a list of items from a database, all in the same inventory
        [Fact]
        public void Get_items()
        {
            //ARRANGE

            Guid invId = new("10000000-0000-0000-0000-000000000001");
            Guid itemId = new("10000000-0000-0000-0000-000000000002");
            Guid itemId2 = new("10000000-0000-0000-0000-000000000003");

            var myInv = new Inventory {Id = invId, name = "testInv", userId = "123"};
                
            var data = new List<Item>
            {

            }.AsQueryable();
            
            var mockSet = new Mock<DbSet<Item>>();
            mockSet.As<IQueryable<Item>>().Setup(m => m.Provider).Returns(data.Provider);
            mockSet.As<IQueryable<Item>>().Setup(m => m.Expression).Returns(data.Expression);
            mockSet.As<IQueryable<Item>>().Setup(m => m.ElementType).Returns(data.ElementType);
            mockSet.As<IQueryable<Item>>().Setup(m => m.GetEnumerator()).Returns(data.GetEnumerator());

            var mockIn = new Mock<IItemService>();
            var testItem = new Item();
            testItem.inventoryId = invId;
            

            //ACT
            
            var dbMock = new Mock<DBContext>();
            dbMock.Setup(x => x.Set<Item>()).Returns(mockSet.Object);
            var myser = new ItemService(dbMock.Object);
            
            var thenew = new Item { Id = itemId, name = "Leopard stripe shirt", inventoryId  = invId, soldDate = DateTime.Now, type = "Clothes"};
            var thenew2 = new Item { Id = itemId2, name = "Grey sandals", inventoryId  = invId, soldDate = DateTime.Now, type = "Clothes"};

            myser.CreateItem(thenew);
            myser.CreateItem(thenew2);

            var myItems = myser.GetItems(invId);
            var icount = myItems.Count;
            
            //ASSERT
            
            Assert.Equal(2, icount);
        }

        //Unit test defined to get the inventory ID from the item passed in
        [Fact]
        public void Get_invID_from_item()
        {
            //ARRANGE

            Guid invId = new("10000000-0000-0000-0000-000000000001");
            Guid itemId = new("10000000-0000-0000-0000-000000000002");
            Guid itemId2 = new("10000000-0000-0000-0000-000000000003");

            var myInv = new Inventory {Id = invId, name = "testInv", userId = "123"};
                
            var data = new List<Item>
            {

            }.AsQueryable();
            
            var mockSet = new Mock<DbSet<Item>>();
            mockSet.As<IQueryable<Item>>().Setup(m => m.Provider).Returns(data.Provider);
            mockSet.As<IQueryable<Item>>().Setup(m => m.Expression).Returns(data.Expression);
            mockSet.As<IQueryable<Item>>().Setup(m => m.ElementType).Returns(data.ElementType);
            mockSet.As<IQueryable<Item>>().Setup(m => m.GetEnumerator()).Returns(data.GetEnumerator());

            var mockIn = new Mock<IItemService>();
            var testItem = new Item();
            testItem.inventoryId = invId;
            

            //ACT
            
            var dbMock = new Mock<DBContext>();
            dbMock.Setup(x => x.Set<Item>()).Returns(mockSet.Object);
            var myser = new ItemService(dbMock.Object);
            
            var thenew = new Item { Id = itemId, name = "Leopard stripe shirt", inventoryId  = invId, soldDate = DateTime.Now, type = "Clothes"};
            var thenew2 = new Item { Id = itemId2, name = "Grey sandals", inventoryId  = invId, soldDate = DateTime.Now, type = "Clothes"};

            myser.CreateItem(thenew);
            myser.CreateItem(thenew2);

            var invIdFound = myser.GetInventoryIdFromItem(itemId);

            //ASSERT
            
            Assert.Equal(invId, invIdFound);

        }

        
        //Unit test defined to get the inventory ID from the item passed in (negative testing)
        [Fact]
        public void Get_invID_from_item_INVALID()
        {
            //ARRANGE

            Guid invId = new("10000000-0000-0000-0000-000000000001");
            Guid itemId = new("10000000-0000-0000-0000-000000000002");
            Guid itemId2 = new("10000000-0000-0000-0000-000000000003");

            var myInv = new Inventory {Id = invId, name = "testInv", userId = "123"};
                
            var data = new List<Item>
            {

            }.AsQueryable();
            
            var mockSet = new Mock<DbSet<Item>>();
            mockSet.As<IQueryable<Item>>().Setup(m => m.Provider).Returns(data.Provider);
            mockSet.As<IQueryable<Item>>().Setup(m => m.Expression).Returns(data.Expression);
            mockSet.As<IQueryable<Item>>().Setup(m => m.ElementType).Returns(data.ElementType);
            mockSet.As<IQueryable<Item>>().Setup(m => m.GetEnumerator()).Returns(data.GetEnumerator());

            var mockIn = new Mock<IItemService>();
            var testItem = new Item();
            testItem.inventoryId = invId;
            

            //ACT
            
            var dbMock = new Mock<DBContext>();
            dbMock.Setup(x => x.Set<Item>()).Returns(mockSet.Object);
            var myser = new ItemService(dbMock.Object);
            
            var thenew = new Item { Id = itemId, name = "Leopard stripe shirt", inventoryId  = invId, soldDate = DateTime.Now, type = "Clothes"};

            myser.CreateItem(thenew);

            var invIdFound = myser.GetInventoryIdFromItem(itemId2);

            //ASSERT
            
            Assert.Equal(Guid.Empty, invIdFound);

        }

    }
    


}


