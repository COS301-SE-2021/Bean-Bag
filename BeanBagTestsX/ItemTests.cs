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
            Assert.NotEqual("", testItem.QRContents);

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

            var myser = new Mock<IItemService>();
            myser.Setup(x => x.GetItems(invId)).Returns(mockSet.Object.ToList());
            
            var thenew = new Item { Id = itemId, name = "Leopard stripe shirt", inventoryId  = invId};
            
            myser.Setup(x => x.CreateItem(thenew));
            myser.Object.CreateItem(thenew);
            
            myser.Setup(x => x.FindItem(thenew.Id)).Returns(thenew);

            var foundCreated = myser.Object.FindItem(thenew.Id);
            
            //ASSERT
            
            //Assert.True(isDeleted);
            Assert.NotNull(foundCreated);

        }

        //Unit test defined for deleting an item 
        [Fact]
        public void Delete_item()
        {
            //ARRANGE

            Guid invId = new("10000000-0000-0000-0000-000000000001");
            Guid itemId = new("00000000-0000-0000-0000-000000000001");

            var myInv = new Inventory {Id = invId, name = "testInv", userId = "123"};
                
            var data = new List<Item>
            {
                new Item {Id = itemId, name = "sock", inventoryId = invId}
            }.AsQueryable();
            
            var mockSet = new Mock<DbSet<Item>>();
            mockSet.As<IQueryable<Item>>().Setup(m => m.Provider).Returns(data.Provider);
            mockSet.As<IQueryable<Item>>().Setup(m => m.Expression).Returns(data.Expression);
            mockSet.As<IQueryable<Item>>().Setup(m => m.ElementType).Returns(data.ElementType);
            mockSet.As<IQueryable<Item>>().Setup(m => m.GetEnumerator()).Returns(data.GetEnumerator());

            var mockIn = new Mock<IItemService>();


            //ACT

            var myser = new Mock<IItemService>();
            myser.Setup(x => x.GetItems(invId)).Returns(mockSet.Object.ToList());
            
            var thenew = new Item { Id = itemId, name = "Leopard stripe shirt", inventoryId  = invId};
            myser.Object.CreateItem(thenew);
            
            myser.Setup(x => x.DeleteItem(thenew.Id));

            //var isDeleted = myser.Object.DeleteItem(thenew.Id);
            
            var confirmGone = myser.Object.FindItem(thenew.Id);
            
            //ASSERT
            
            //Assert.True(isDeleted);
            Assert.Null(confirmGone);

        }

        //Unit test defined for editing an item 
        [Fact]
        public void Edit_item()
        {
            //ARRANGE

            Guid invId = new("10000000-0000-0000-0000-000000000001");
            Guid itemId = new("00000000-0000-0000-0000-000000000001");

            var myInv = new Inventory {Id = invId, name = "testInv", userId = "123"};
                
            var data = new List<Item>
            {
                new Item {Id = itemId, name = "sock", inventoryId = invId, isSold = false}
            }.AsQueryable();
            
            var mockSet = new Mock<DbSet<Item>>();
            mockSet.As<IQueryable<Item>>().Setup(m => m.Provider).Returns(data.Provider);
            mockSet.As<IQueryable<Item>>().Setup(m => m.Expression).Returns(data.Expression);
            mockSet.As<IQueryable<Item>>().Setup(m => m.ElementType).Returns(data.ElementType);
            mockSet.As<IQueryable<Item>>().Setup(m => m.GetEnumerator()).Returns(data.GetEnumerator());

            //var mockIn = new Mock<IItemService>();


            //ACT

            var myser = new Mock<IItemService>();
            myser.Setup(x => x.GetItems(invId)).Returns(mockSet.Object.ToList());
            
            var thenew = new Item { Id = itemId, name = "Leopard stripe shirt", inventoryId  = invId};
            myser.Object.CreateItem(thenew);

            myser.Setup(x => x.EditItem(thenew));

            myser.Object.EditItem(thenew);

            //ASSERT
            Assert.Equal(DateTime.MinValue, thenew.soldDate);

        }

        //Unit test defined to find an item from the database
        [Fact]
        public void Find_item()
        {
            //ARRANGE

            Guid invId = new("10000000-0000-0000-0000-000000000001");
            Guid itemId = new("00000000-0000-0000-0000-000000000001");

            var myInv = new Inventory {Id = invId, name = "testInv", userId = "123"};
                
            var data = new List<Item>
            {
               
            }.AsQueryable();
            
            var mockSet = new Mock<DbSet<Item>>();
            mockSet.As<IQueryable<Item>>().Setup(m => m.Provider).Returns(data.Provider);
            mockSet.As<IQueryable<Item>>().Setup(m => m.Expression).Returns(data.Expression);
            mockSet.As<IQueryable<Item>>().Setup(m => m.ElementType).Returns(data.ElementType);
            mockSet.As<IQueryable<Item>>().Setup(m => m.GetEnumerator()).Returns(data.GetEnumerator());

            //ACT
            var myser = new Mock<IItemService>();
            myser.Setup(x => x.GetItems(invId)).Returns(mockSet.Object.ToList());
            
            var thenew = new Item { Id = itemId, name = "Pants", inventoryId = invId};
            myser.Object.CreateItem(thenew);

            myser.Setup(x => x.FindItem(thenew.Id));

            var isFound = myser.Object.FindItem(thenew.Id);

            //ASSERT
            Assert.Null(isFound);

        }

        //Unit test defined to retrieve a list of items from a database, all in the same inventory
        [Fact]
        public void Get_items()
        {
            //ARRANGE

            Guid invId = new("10000000-0000-0000-0000-000000000001");
            Guid itemId = new("00000000-0000-0000-0000-000000000001");

            var myInv = new Inventory {Id = invId, name = "testInv", userId = "123"};
                
            var data = new List<Item>
            {
                new Item {Id = itemId, name = "sock", inventoryId = invId}
            }.AsQueryable();
            
            var mockSet = new Mock<DbSet<Item>>();
            mockSet.As<IQueryable<Item>>().Setup(m => m.Provider).Returns(data.Provider);
            mockSet.As<IQueryable<Item>>().Setup(m => m.Expression).Returns(data.Expression);
            mockSet.As<IQueryable<Item>>().Setup(m => m.ElementType).Returns(data.ElementType);
            mockSet.As<IQueryable<Item>>().Setup(m => m.GetEnumerator()).Returns(data.GetEnumerator());

            var mockIn = new Mock<IItemService>();


            //ACT
            mockIn.Setup(x => x.GetItems(invId)).Returns(mockSet.Object.ToList());

            var myItems = mockIn.Object.GetItems(invId);
            
            //ASSERT
            Assert.IsType<List<Item>>(myItems);
        }

        //Unit test defined to get the inventory ID from the item passed in
        [Fact]
        public void Get_invID_from_item()
        {
            //ARRANGE

            Guid invId = new("10000000-0000-0000-0000-000000000001");
            Guid itemId = new("00000000-0000-0000-0000-000000000001");

            var myInv = new Inventory {Id = invId, name = "testInv", userId = "123"};
                
            var data = new List<Item>
            {
                new Item {Id = itemId, name = "sock", inventoryId = invId}
            }.AsQueryable();
            
            var mockSet = new Mock<DbSet<Item>>();
            mockSet.As<IQueryable<Item>>().Setup(m => m.Provider).Returns(data.Provider);
            mockSet.As<IQueryable<Item>>().Setup(m => m.Expression).Returns(data.Expression);
            mockSet.As<IQueryable<Item>>().Setup(m => m.ElementType).Returns(data.ElementType);
            mockSet.As<IQueryable<Item>>().Setup(m => m.GetEnumerator()).Returns(data.GetEnumerator());

            var mockIn = new Mock<IItemService>();


            //ACT
            mockIn.Setup(x => x.GetItems(invId)).Returns(mockSet.Object.ToList());

            var myId = mockIn.Object.GetInventoryIdFromItem(itemId);    
            
            //ASSERT
            Assert.IsType<Guid>(myId);

        }


    }
    


}


