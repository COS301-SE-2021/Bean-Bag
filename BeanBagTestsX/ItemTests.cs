using System;
using System.Collections.Generic;
using System.Linq;
using BeanBag.Database;
using BeanBag.Models;
using BeanBag.Services;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;

namespace BeanBagTestsX
{



    public class ItemTests
    {

        // Unit test defined for adding a QR code to an item using the azure function
        [Fact]
        public void Add_QRitem_with_valid_item()
        {
            //ARRANGE
            var mockIn = new Mock<IItemService>();


            //ACT


            //ASSERT


        }

    }
    


}


