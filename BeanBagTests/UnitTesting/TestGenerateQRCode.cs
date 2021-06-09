using System;
using BeanBag.Controllers;
using BeanBag.Models;
using Xunit;

namespace BeanBagUnitTesting.UnitTesting
{
    public class TestGenerateQrCode 
    {
        
        /*---------------------------------- POSITIVE TESTING----------------------------------------- */
               
        /* Unit test for generating a QR Code with valid itemID input expecting success
         * If the function returns true the coupling was achieved successfully
         * If the function returns false the coupling failed*/
        
        [Fact] 
        public void Test_QRCode_Generation_Successful_Valid_ItemID() 
        {
            //Arrange
            String ItemId = "7617671671";
            
            //Act
            QrCodeController qr = new QrCodeController();
            var qrCodeModel = new QrCodeModel();
            qr.GenerateQrCode(ItemId, qrCodeModel);
            
            //Assert
            Assert.NotNull(qrCodeModel.QrCodeNumber);
        }
        
                
        /*---------------------------------- NEGATIVE TESTING-----------------------------------------*/
        
        /* Unit test for generating a QR Code with invalid itemID input expecting failure
         * If the function returns true the coupling was achieved successfully
         * If the function returns false the coupling failed*/
        
        [Fact]
        public void Test_QRCode_Generation_Failure_Invalid_ItemID()
        {
            //Arrange
            String ItemId = "7617671671";
            QrCodeController qr = new QrCodeController();
            var qrCodeModel = new QrCodeModel();
            
            //Act
            qr.GenerateQrCode(ItemId, qrCodeModel);
            
            //Assert
            Assert.Null(qrCodeModel.QrCodeNumber); //fails because database is needed to query if item exists
        }
    }
}