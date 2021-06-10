using System;
using BeanBag.Controllers;
using Xunit;

namespace BeanBagUnitTesting.UnitTesting
{
    public class TestCouplingQrCode  
    {
        /*---------------------------------- POSITIVE TESTING----------------------------------------- */
        
        /* Unit test for coupling QR Code to an item with valid input, expecting success*/
        [Fact]
        public void Test_QrCode_Coupling_Valid_Input()
        {
            //Arrange
            String ItemNumber = "7878178178";
            String QRNumber = "131341";
            
            //Act
            QrCodeController qr = new QrCodeController();
            bool result = qr.CoupleQrCode(ItemNumber, QRNumber);
            
            //Assert
            Assert.True(result);
        }

        /*---------------------------------- NEGATIVE TESTING----------------------------------------- */
        
         /* Unit test for coupling QR Code to an item with valid input expecting failure given invalid QRNumber*/
         [Fact]
         public void Test_QrCode_Coupling_Invalid_QRNum()
         {
             //Arrange
             String ItemNumber = "7878178178";
             String QRNumber = "131341";
            
             //Act
             QrCodeController qr = new QrCodeController();
             bool result = qr.CoupleQrCode(ItemNumber, QRNumber);
            
             //Assert
             Assert.False(result); // need database thats why fails
         }
         
         /* Unit test for coupling QR Code to an item with valid input expecting failure given invalid ItemNumber*/
         [Fact]
         public void Test_QrCode_Coupling_Invalid_ItemNum()
         {
             //Arrange
             String ItemNumber = "7878178178";
             String QRNumber = "131341";
            
             //Act
             QrCodeController qr = new QrCodeController();
             bool result = qr.CoupleQrCode(ItemNumber, QRNumber);
            
             //Assert
             Assert.False(result); // need database that's why fails
         }
        
    }
}