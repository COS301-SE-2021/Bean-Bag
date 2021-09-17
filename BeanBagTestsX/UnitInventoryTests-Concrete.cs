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
                
                mySer.CreateInventory(myTest);

                var myInv = mySer.FindInventory(theId2);
                
                Assert.Equal(theId2, myInv.Id);
            }
        }
        
        

    }

    


}


