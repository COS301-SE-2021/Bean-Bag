using System;
using System.Collections.Generic;
using BeanBag.Database;
using BeanBag.Models;
using BeanBag.Services;
using Moq;
using Xunit;

namespace BeanBagTestsX
{
    public class InventoryTests
    {
        private readonly InventoryService _iut;
        private readonly Mock<DBContext> _invMock = new Mock<DBContext>();
        
        public InventoryTests()
        {
            _iut = new InventoryService(_invMock.Object);
            
        }
        
        
        [Fact]
        public void Get_user_inventories_with_valid_id()
        {
            //ARRANGE
            string theId = "1234";
            _invMock.Setup(x => x.Find(theId)).Returns(_iut.GetInventories(theId));


            //ACT
            var invs = _iut.GetInventories(theId);

            //ASSERT
            Assert.Equal(new List<Inventory>(),invs );

        }
    }
}

//var exception = Assert.Throws<Exception>(Act);
