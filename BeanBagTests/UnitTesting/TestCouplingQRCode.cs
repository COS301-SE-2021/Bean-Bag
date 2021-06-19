using BeanBag.Controllers;
using Xunit;

namespace BeanBagUnitTesting.UnitTesting
{
    public class TestCouplingQrCode  
    {
        //---------------------------------- POSITIVE TESTING----------------------------------------- 
        
        // Unit test for coupling QR Code to an item with valid input, expecting success.
        [Fact]
        public void Test_CoupleQrCode_ValidInput_True()
        {
            // Arrange.
            const string itemNumber = "7878178178";
            const string qRNumber = "131341";

            // Act.
            var qr = new QrCodeController();
            bool result = qr.CoupleQrCode(itemNumber, qRNumber);
            
            // Assert.
            Assert.True(result);
        }

        //---------------------------------- NEGATIVE TESTING----------------------------------------- 
        
         // Unit test for coupling QR Code to an item with valid input expecting failure given invalid QRNumber.
         [Fact]
         public void Test_CoupleQrCode_InvalidQRCode_False()
         {
             // Arrange.
             const string itemNumber = "7878178178";
             const string qRNumber = "131341";
            
             // Act.
             var qr = new QrCodeController();
             bool result = qr.CoupleQrCode(itemNumber, qRNumber);
            
             // Assert.
             Assert.False(result); 
         }
         
         // Unit test for coupling QR Code to an item with valid input expecting failure given invalid ItemNumber.
         [Fact]
         public void Test_CoupleQrCode_InvalidItemNumber_False()
         {
             // Arrange.
             const string itemNumber = "7878178178";
             const string qRNumber = "131341";
            
             // Act.
             var qr = new QrCodeController();
             bool result = qr.CoupleQrCode(itemNumber, qRNumber);
            
             // Assert.
             Assert.False(result); 
         }
        
    }
}