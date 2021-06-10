using System;
using BeanBag.Controllers;
using Xunit;

namespace BeanBagUnitTesting.UnitTesting
{
    public class TestGenerateQrCode 
    {
        /*---------------------------------- POSITIVE TESTING----------------------------------------- */
               
        /* Unit test for generating a QR Code with valid itemID input, expecting success*/
        [Fact] 
        public void Test_QRCode_Generation_Successful_Valid_ItemID() 
        {
            //Arrange
            var itemId = new Guid().ToString() ;
            
            //Act
            var qr = new QrCodeController();
            
            //Assert
           Assert.True(qr.GenerateQrCode(itemId));
        }

        /*---------------------------------- NEGATIVE TESTING-----------------------------------------*/
        
        /* Unit test for generating a QR Code with invalid itemID input, expecting failure*/
        [Fact]
        public void Test_QRCode_Generation_Failure_Invalid_ItemID()
        {
            //Arrange
            const string itemId = "InvalidString123";
            var qr = new QrCodeController();
            
            //Act
            void Act() => qr.GenerateQrCode(itemId);

            //Assert
            var exception = Assert.Throws<Exception>(Act);
            Assert.Equal("QRCode generation failed. ItemID string is invalid.", exception.Message);
        }

        /* Unit test for generating a QR Code with invalid null input, expecting failure*/
        [Fact]
        public void Test_QRCode_Generation_Failure_Null_ItemID()
        {
            //Arrange
            var qr = new QrCodeController();
            
            //Act
            Action act =() => qr.GenerateQrCode(null);

            //Assert
            var exception = Assert.Throws<Exception>(act);
            Assert.Equal("QRCode generation failed. ItemID input is null.", exception.Message);
        }
    }
}