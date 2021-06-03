using BeanBag.Controllers;
using System;
using Xunit;

namespace BeanBagUnitTesting
{
    public class UnitTests
    {
        /*
         * Unit test for coupling QR Code to an item 
         * If the function returns true the coupling was achieved successfully
         * If the function returns false the coupling failed
         */
        [Fact]
        public void TestQRCodeCoupling()
        {
            //Mock data
            String ItemNumber = "7878178178";
            String QRNumber = "131341";

            // Arrange
            QRCodeController qr = new QRCodeController();

            //Act
            bool result = qr.coupleQRCode(ItemNumber, QRNumber);

            //Assert
            Assert.True(result);
        }

        /*
        * Unit test for generating a QR Code 
        * If the function returns true the coupling was achieved successfully
        * If the function returns false the coupling failed
        */
        [Fact]
        public void TestQRCodeGeneration()
        {
            //Mock data
            String ItemId = "7617671671";

            //Arrange
            QRCodeController qr = new QRCodeController();

            //Act
            bool result = qr.generateQRCode(ItemId);

            //Assert
            Assert.True(result);
        }

        /*
         * Unit test for logging in
         */


        /*
        * Unit test for logging in
        */

        /*
         * Unit test for create/add item to database
         */

    }
}
