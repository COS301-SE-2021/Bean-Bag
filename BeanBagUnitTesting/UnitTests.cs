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
            String ItemNumber = "7878178178";
            String QRNumber = "dummyQR";

            QRCodeController qr = new QRCodeController();
            bool result = qr.coupleQRCode(ItemNumber, QRNumber);
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

            QRCodeController qr = new QRCodeController();
            String ItemId = "7617671671716";

            bool result = qr.generateQRCode(ItemId);
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
