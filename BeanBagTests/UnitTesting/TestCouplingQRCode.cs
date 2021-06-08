using System;
using BeanBag.Controllers;
using Xunit;

namespace BeanBagUnitTesting.UnitTesting
{
    public class TestCouplingQrCode  
    {
        /*---------------------------------- POSITIVE TESTING----------------------------------------- */
        
        /* Unit test for coupling QR Code to an item with valid input expecting success
         * If the function returns true the coupling was achieved successfully
         * If the function returns false the coupling failed*/
        [Fact]
        public void Test_QrCode_Coupling_Valid_Input()
        {
            String ItemNumber = "7878178178";
            String QRNumber = "131341";  
            QrCodeController qr = new QrCodeController();
            bool result = qr.CoupleQrCode(ItemNumber, QRNumber);
            Assert.True(result);
        }

        /*---------------------------------- NEGATIVE TESTING----------------------------------------- */
        
         /* Unit test for coupling QR Code to an item with valid input expecting failure given invalid QRNumber
          * If the function returns true the coupling was achieved successfully
          * If the function returns false the coupling failed*/
         [Fact]
         public void Test_QrCode_Coupling_Invalid_QRNum()
         {
             String ItemNumber = "7878178178";
             String QRNumber = "131341";
             QrCodeController qr = new QrCodeController();
             bool result = qr.CoupleQrCode(ItemNumber, QRNumber);
             Assert.False(result);
         }
         
         /* Unit test for coupling QR Code to an item with valid input expecting failure given invalid ItemNumber
          * If the function returns true the coupling was achieved successfully
          * If the function returns false the coupling failed */
         [Fact]
         public void Test_QrCode_Coupling_Invalid_ItemNum()
         {
             String ItemNumber = "7878178178";
             String QRNumber = "131341";
             QrCodeController qr = new QrCodeController();
             bool result = qr.CoupleQrCode(ItemNumber, QRNumber);
             Assert.False(result);
         }
         
         
    }
}