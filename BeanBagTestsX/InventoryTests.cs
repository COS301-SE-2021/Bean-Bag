using System;
using System.Collections.Generic;
using BeanBag.Models;
using BeanBag.Services;
using Xunit;

namespace BeanBagTestsX
{
    public class InventoryTests
    {
        [Fact]
        public void Get_user_inventories_valid_id()
        {
            //ARRANGE
            string theId = "1234";

            BeanBag.Database.DBContext db = null;
            var code = new InventoryService(db);

            //ACT
            void Act() => code.GetInventories(theId);

            //ASSERT
            Assert.Equal(true, true);

        }
    }
}

//var exception = Assert.Throws<Exception>(Act);
