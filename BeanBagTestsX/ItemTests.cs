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
            var testItem = new Item();
            var mockIn = new Mock<IItemService>();
            
            //ACT
            mockIn.Object.AddQRItem(testItem);

            //ASSERT
            Assert.NotEqual("", testItem.QRContents);

        }

        //Unit test defined for creating an item
        [Fact]
        public void Create_item()
        {
            //ARRANGE
            var mockIn = new Mock<IItemService>();


            //ACT


            //ASSERT


        }

        //Unit test defined for deleting an item 
        [Fact]
        public void Delete_item()
        {
            //ARRANGE
            var mockIn = new Mock<IItemService>();


            //ACT


            //ASSERT


        }

        //Unit test defined for editing an item 
        [Fact]
        public void Edit_item()
        {
            //ARRANGE
            var mockIn = new Mock<IItemService>();


            //ACT


            //ASSERT


        }

        //Unit test defined to find an item from the database
        [Fact]
        public void Find_item()
        {
            //ARRANGE
            var mockIn = new Mock<IItemService>();


            //ACT


            //ASSERT


        }

        //Unit test defined to retrieve a list of items from a database, all in the same inventory
        [Fact]
        public void Get_items()
        {
            //ARRANGE
            var mockIn = new Mock<IItemService>();


            //ACT


            //ASSERT


        }

        //Unit test defined to get the inventory ID from the item passed in
        [Fact]
        public void Get_invID_from_item()
        {
            //ARRANGE
            var mockIn = new Mock<IItemService>();


            //ACT


            //ASSERT


        }


    }
    


}


