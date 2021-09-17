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
    


    public class UnitInventoryTestsConcrete : UnitInventoryTests
    {
        
        public UnitInventoryTestsConcrete() : base(new DbContextOptionsBuilder<DBContext>()
            .UseSqlite("Filename=Test.db").Options)
        {
            
        }

        [Fact]
        public void Create_inventory_valid()
        {
            //ARRANGE
            using (var context = new DBContext(ContextOptions))
            {
                var mySer = new InventoryService(context);

                var chars = "0123456789";
                var stringChars = new char[5];
                var random = new Random();

                for (var i = 0; i < stringChars.Length; i++)
                {
                    stringChars[i] = chars[random.Next(chars.Length)];
                }

                var finalString = new String(stringChars);

                var myGuidEnd = finalString;

                var u2 = finalString.Substring(0, 4);
            
                Guid theId2 = new("00000000-0000-0000-0000-0000000" + myGuidEnd);
                
                var myTest = new Inventory { Id = theId2, name = "Unit test inventory", userId = u2 , publicToTenant = false};
                
                //ACT
                mySer.CreateInventory(myTest);

                var myInv = mySer.FindInventory(theId2);
                
                //ASSERT
                Assert.Equal(theId2, myInv.Id);
                mySer.DeleteInventory(theId2, u2);

            }
        }
        
        
        // Unit test defined for the get user inventories (positive testing)
        [Fact]
        public void Get_user_inventories_with_valid_id()
        {
            //ARRANGE
            using (var context = new DBContext(ContextOptions))
            {
                var mySer = new InventoryService(context);

                var chars = "0123456789";
                var stringChars = new char[5];
                var random = new Random();

                for (var i = 0; i < stringChars.Length; i++)
                {
                    stringChars[i] = chars[random.Next(chars.Length)];
                }

                var finalString = new String(stringChars);

                var myGuidEnd = finalString;

                var u2 = finalString.Substring(0, 4);

                Guid theId2 = new("00000000-0000-0000-0000-0000000" + myGuidEnd);
                Guid theId3 = new("00000000-0000-0000-0000-0010000" + myGuidEnd);

                var myTest = new Inventory{Id = theId2, name = "Unit test inventory", userId = u2, publicToTenant = false};
                var myTest2 = new Inventory{Id = theId3, name = "Unit test inventory 2", userId = u2, publicToTenant = false};

                //ACT
                mySer.CreateInventory(myTest);
                mySer.CreateInventory(myTest2);

                var myInv = mySer.GetInventories(u2);

                var invCount = myInv.Count;

                //ASSERT
                Assert.Equal(2, invCount);
                mySer.DeleteInventory(theId2, u2);
                mySer.DeleteInventory(theId3, u2);
            }
        }
        
        
        // Unit test defined for the delete inventories (negative testing)
        [Fact]
        public void Delete_inventory_wrong_user_id()
        {
            //ARRANGE
            using (var context = new DBContext(ContextOptions))
            {
                var mySer = new InventoryService(context);

                var chars = "0123456789";
                var stringChars = new char[5];
                var random = new Random();

                for (var i = 0; i < stringChars.Length; i++)
                {
                    stringChars[i] = chars[random.Next(chars.Length)];
                }

                var finalString = new String(stringChars);

                var myGuidEnd = finalString;

                var u2 = finalString.Substring(0, 4);
                var u3 = "0000";

                Guid theId2 = new("00000000-0000-0000-0000-0000000" + myGuidEnd);
                Guid theId3 = new("00000000-0000-0000-0000-0010000" + myGuidEnd);
                Guid theId4 = new("00000000-0000-0000-0000-0011000" + myGuidEnd);

                var myTest = new Inventory{Id = theId2, name = "Unit test inventory", userId = u2, publicToTenant = false};
                var myTest2 = new Inventory{Id = theId3, name = "Unit test inventory 2", userId = u2, publicToTenant = false};

                //ACT
                mySer.CreateInventory(myTest);
                mySer.CreateInventory(myTest2);

                var myCheck = mySer.DeleteInventory(theId2, u3);

                //ASSERT
                Assert.False(myCheck);
                mySer.DeleteInventory(theId3, u2);
            }
        }
        
        // Unit test defined for the edit inventories (negative testing)
        [Fact]
        public void Edit_inventory_valid()
        {
            //ARRANGE
            using (var context = new DBContext(ContextOptions))
            {
                var mySer = new InventoryService(context);

                var chars = "0123456789";
                var stringChars = new char[5];
                var random = new Random();

                for (var i = 0; i < stringChars.Length; i++)
                {
                    stringChars[i] = chars[random.Next(chars.Length)];
                }

                var finalString = new String(stringChars);

                var myGuidEnd = finalString;

                var u2 = finalString.Substring(0, 4);
                var u3 = "0000";

                Guid theId2 = new("00000000-0000-0000-0000-0000000" + myGuidEnd);
                Guid theId3 = new("00000000-0000-0000-0000-0010000" + myGuidEnd);
                Guid theId4 = new("00000000-0000-0000-0000-0011000" + myGuidEnd);

                var myTest = new Inventory{Id = theId2, name = "Unit test inventory", userId = u2, publicToTenant = false};
                var myTest2 = new Inventory{Id = theId3, name = "Unit test inventory 2", userId = u2, publicToTenant = false};

                //ACT
                mySer.CreateInventory(myTest);
                mySer.CreateInventory(myTest2);

                var myCheck = mySer.EditInventory(u2, myTest);

                //ASSERT
                Assert.True(myCheck);
                mySer.DeleteInventory(theId3, u2);
            }
        }
        
        
        // Unit test defined for the edit inventories (negative testing)
        [Fact]
        public void Edit_inventory_invalid_user_id()
        {
            //ARRANGE
            using (var context = new DBContext(ContextOptions))
            {
                var mySer = new InventoryService(context);

                var chars = "0123456789";
                var stringChars = new char[5];
                var random = new Random();

                for (var i = 0; i < stringChars.Length; i++)
                {
                    stringChars[i] = chars[random.Next(chars.Length)];
                }

                var finalString = new String(stringChars);

                var myGuidEnd = finalString;

                var u2 = finalString.Substring(0, 4);
                var u3 = "0000";

                Guid theId2 = new("00000000-0000-0000-0000-0000000" + myGuidEnd);
                Guid theId3 = new("00000000-0000-0000-0000-0010000" + myGuidEnd);
                Guid theId4 = new("00000000-0000-0000-0000-0011000" + myGuidEnd);

                var myTest = new Inventory{Id = theId2, name = "Unit test inventory", userId = u2, publicToTenant = false};
                var myTest2 = new Inventory{Id = theId3, name = "Unit test inventory 2", userId = u2, publicToTenant = false};

                //ACT
                mySer.CreateInventory(myTest);
                mySer.CreateInventory(myTest2);

                var myCheck = mySer.EditInventory(u3, myTest);

                //ASSERT
                Assert.False(myCheck);
                mySer.DeleteInventory(theId3, u2);
            }
        }


    }

    


}


