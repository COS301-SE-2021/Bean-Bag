using System;
using BeanBag.Controllers;
using Xunit;

namespace BeanBagUnitTesting.UnitTesting
{
    public class TestGenerateQrCode 
    {
        //---------------------------------- POSITIVE TESTING----------------------------------------- 
               
        // Unit test for generating a QR Code with valid itemID input, expecting success.
        [Fact] 
        public void Test_QRCode_Generation_Successful_Valid_ItemID() 
        {
            //Arrange
            var itemId = new Guid().ToString() ;
            
            //Act
            var code = new QrCodeController();
            
            //Assert
           Assert.True(code.GenerateQrCode(itemId));
        }

        //---------------------------------- NEGATIVE TESTING-----------------------------------------
        
        // Unit test for generating a QR Code with invalid itemID input, expecting failure.
        [Fact]
        public void Test_QRCode_Generation_Failure_Invalid_ItemID()
        {
            // Arrange.
            const string itemId = "InvalidString123";
            var code = new QrCodeController();
            
            // Act.
            void Act() => code.GenerateQrCode(itemId);

            // Assert.
            var exception = Assert.Throws<Exception>(Act);
            Assert.Equal("QRCode generation failed. ItemID string length is invalid.", exception.Message);
        }

        // Unit test for generating a QR Code with invalid null input, expecting failure. 
        [Fact]
        public void Test_QRCodeGeneration_NullItemID_ThrowsException()
        {
            // Arrange.
            var code = new QrCodeController();
            
            // Act.
            void Act() => code.GenerateQrCode(null);

            // Assert.
            var exception = Assert.Throws<Exception>(Act);
            Assert.Equal("QRCode generation failed. ItemID input is null.", exception.Message);
        }
    }
}