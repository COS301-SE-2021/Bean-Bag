using System;
using BeanBag.Database;
using BeanBag.Models;
using BeanBag.Services;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;

namespace BeanBagUnitTests
{

    public class UnitItemTestsConcrete : UnitInventoryTests
    {
        
        public UnitItemTestsConcrete() : base(new DbContextOptionsBuilder<DBContext>()
            .UseSqlite("Filename=Test.db").Options)
        {
            
        }
        
        // Unit test defined for adding a QR code to an item using the azure function
        [Fact]
        public void Add_QR_item_with_valid_item()
        {
            //ARRANGE
            var testItem = new Item();
            var mockIn = new Mock<IItemService>();
            
            //ACT
            mockIn.Object.AddQrItem(testItem);

            //ASSERT
            Assert.NotEqual("", testItem.QRCodeLink);

        }

        // Unit test defined to create an item
        [Fact]
        public void Create_item_valid()
        {
            using (var context = new DBContext(ContextOptions))
            {
                //ARRANGE

                Guid invId = new("10000000-0000-0000-0000-000000000001");
                Guid itemId = new("10000000-0000-0000-0000-000000000002");

                var myInv = new Inventory {Id = invId, name = "testInv", userId = "123"};
                var myinvser = new InventoryService(context);
                myinvser.CreateInventory(myInv);
                
                //ACT
                
                var myser = new ItemService(context);
                
                var thenew = new Item { Id = itemId, name = "Leopard stripe shirt", inventoryId  = myInv.Id, soldDate = DateTime.Now, type = "Clothes"};

                myser.CreateItem(thenew);
                var foundItem = myser.FindItem(thenew.Id);

                //ASSERT
                
                Assert.Equal(itemId, foundItem.Id);
                myinvser.DeleteInventory(myInv.Id, "123");
                myser.DeleteItem(itemId);
            }
            
        }
        
        
        // Unit test defined to delete an item (positive testing)
        [Fact]
        public void Delete_item_valid()
        {
            using (var context = new DBContext(ContextOptions))
            {
                //ARRANGE

                Guid invId = new("10000000-0000-0000-0000-000000000001");
                Guid itemId = new("10000000-0000-0000-0000-000000000002");

                var myInv = new Inventory {Id = invId, name = "testInv", userId = "123"};
                var myinvser = new InventoryService(context);
                myinvser.CreateInventory(myInv);
                
                //ACT
                
                var myser = new ItemService(context);
                
                var thenew = new Item { Id = itemId, name = "Leopard stripe shirt", inventoryId  = myInv.Id, soldDate = DateTime.Now, type = "Clothes"};

                myser.CreateItem(thenew);

                var canDelt = myser.DeleteItem(thenew.Id);

                //ASSERT
                
                Assert.True(canDelt);
                myinvser.DeleteInventory(myInv.Id, "123");
            }
            
        }
        
        // Unit test defined to delete an item (negative testing)
        [Fact]
        public void Delete_item_invalid_item_id()
        {
            using (var context = new DBContext(ContextOptions))
            {
                //ARRANGE

                Guid invId = new("10000000-0000-0000-0000-000000000001");
                Guid itemId = new("10000000-0000-0000-0000-000000000002");

                var myInv = new Inventory {Id = invId, name = "testInv", userId = "123"};
                var myinvser = new InventoryService(context);
                myinvser.CreateInventory(myInv);
                
                //ACT
                
                var myser = new ItemService(context);
                
                var thenew = new Item { Id = itemId, name = "Leopard stripe shirt", inventoryId  = myInv.Id, soldDate = DateTime.Now, type = "Clothes"};

                myser.CreateItem(thenew);

                var canDelt = myser.DeleteItem(invId);

                //ASSERT
                
                Assert.False(canDelt);
                myinvser.DeleteInventory(myInv.Id, "123");
            }
            
        }
        
        // Unit test defined to edit an item (positive testing)
        [Fact]
        public void Edit_item_valid()
        {
            using (var context = new DBContext(ContextOptions))
            {
                //ARRANGE

                Guid invId = new("10000000-0000-0000-0000-000000000001");
                Guid itemId = new("10000000-0000-0000-0000-000000000002");

                var myInv = new Inventory {Id = invId, name = "testInv", userId = "123"};
                var myinvser = new InventoryService(context);
                myinvser.CreateInventory(myInv);
                
                //ACT
                
                var myser = new ItemService(context);
                
                var thenew = new Item { Id = itemId, name = "Leopard stripe shirt", inventoryId  = myInv.Id, soldDate = DateTime.Now, type = "Clothes", isSold = false};

                myser.CreateItem(thenew);

                myser.EditItem(thenew);

                //ASSERT
                
                Assert.Equal(DateTime.MinValue, thenew.soldDate);
                myser.DeleteItem(thenew.Id);
                myinvser.DeleteInventory(myInv.Id, "123");
            }
            
        }
        
        // Unit test defined to get items (positive testing)
        [Fact]
        public void Get_items_valid()
        {
            using (var context = new DBContext(ContextOptions))
            {
                //ARRANGE

                Guid invId = new("10000000-0000-0000-0000-000000000001");
                Guid itemId = new("10000000-0000-0000-0000-000000000002");
                Guid itemId2 = new("10000000-0000-0000-0000-000000000003");

                var myInv = new Inventory {Id = invId, name = "testInv", userId = "123"};
                var myinvser = new InventoryService(context);
                myinvser.CreateInventory(myInv);
                
                //ACT
                
                var myser = new ItemService(context);
                
                var thenew = new Item { Id = itemId, name = "Leopard stripe shirt", inventoryId  = myInv.Id, type = "Clothes", isSold = false};
                var thenew2 = new Item { Id = itemId2, name = "Black vans", inventoryId  = myInv.Id, type = "Clothes", isSold = false};

                myser.CreateItem(thenew);
                myser.CreateItem(thenew2);

                var myList = myser.GetItems(myInv.Id);
                var icount = myList.Count;
                
                //ASSERT
                
                Assert.Equal(2, icount);
                Assert.Equal("Black vans", myList[1].name);
                myser.DeleteItem(thenew.Id);
                myser.DeleteItem(thenew2.Id);
                myinvser.DeleteInventory(myInv.Id, "123");
            }
            
        }
        
        // Unit test defined to get inventory Id from item (positive testing)
        [Fact]
        public void Get_invId_from_item_valid()
        {
            using (var context = new DBContext(ContextOptions))
            {
                //ARRANGE

                Guid invId = new("10000000-0000-0000-0000-000000000001");
                Guid itemId = new("10000000-0000-0000-0000-000000000002");

                var myInv = new Inventory {Id = invId, name = "testInv", userId = "123"};
                var myinvser = new InventoryService(context);
                myinvser.CreateInventory(myInv);
                
                //ACT
                
                var myser = new ItemService(context);
                
                var thenew = new Item { Id = itemId, name = "Leopard stripe shirt", inventoryId  = myInv.Id, type = "Clothes", isSold = false};

                myser.CreateItem(thenew);

                var myid = myser.GetInventoryIdFromItem(itemId);
                
                //ASSERT
                
                Assert.Equal(invId, myid);
                myser.DeleteItem(thenew.Id);
                myinvser.DeleteInventory(myInv.Id, "123");
            }
            
        }
        
        
        // Unit test defined to get inventory Id from item (positive testing)
        [Fact]
        public void Get_invId_from_item_invalid_item_id()
        {
            using (var context = new DBContext(ContextOptions))
            {
                //ARRANGE

                Guid invId = new("10000000-0000-0000-0000-000000000001");
                Guid itemId = new("10000000-0000-0000-0000-000000000002");

                var myInv = new Inventory {Id = invId, name = "testInv", userId = "123"};
                var myinvser = new InventoryService(context);
                myinvser.CreateInventory(myInv);
                
                //ACT
                
                var myser = new ItemService(context);
                
                var thenew = new Item { Id = itemId, name = "Leopard stripe shirt", inventoryId  = myInv.Id, type = "Clothes", isSold = false};

                myser.CreateItem(thenew);

                var myid = myser.GetInventoryIdFromItem(invId);
                
                //ASSERT
                
                Assert.Equal(Guid.Empty, myid);
                myser.DeleteItem(thenew.Id);
                myinvser.DeleteInventory(myInv.Id, "123");
            }
            
        }
        
        
    }


}


