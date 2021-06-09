using BeanBag.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using QRCoder;

namespace BeanBag.Controllers
{
    /* This class is the controller class for the QRCode Model, this class is responsible for 
     * implementing the functions of the QRCode class and returning data to the QR page*/
    public class QrCodeController : Controller
    {
        
        /* This function generates a QRCode and gets the itemDetails as the parameter. Memory stream helps write from
         * and to the memory. Data and input text is used to generate the QR Code's actual 'DATA'. A new instance of
         * the QR code compiled data of text and string input is created.bitmap maps our string and data to image bits
         * which are then 'drawn', thereafter the generated QRCode variable is set.*/
        public void GenerateQrCode(string input, QrCodeModel qrModel)
        { 
            string itemName = "Item: Chair\n", itemInventory = "Inventory: Furniture Inventory\n",iType = "Type: Furniture\n";
            string  itemId = itemName + itemInventory + iType;
            // Dummy -- Mocking out backend to test [mocking recognised data that will come from AI model function]
            // write code here to check if item is actually in the database (Need for unit testing) throw error if isnt
            // Will need to query the details of the item from the database also
            
            using (MemoryStream ms = new MemoryStream())
            {
                QRCodeGenerator qRCodeGenerator = new QRCodeGenerator();
                QRCodeData qRCodeData = qRCodeGenerator.CreateQrCode(itemId, QRCodeGenerator.ECCLevel.Q);
                QRCode qRCode = new QRCode(qRCodeData);
                
                using (Bitmap bitmap = qRCode.GetGraphic(20))
                {
                    bitmap.Save(ms, ImageFormat.Png);
                    ViewBag.QRCode = "data:image/png;base64," + Convert.ToBase64String(ms.ToArray());
                    qrModel.QrCodeNumber = qRCode.ToString();
                    CoupleQrCode(itemId, qrModel.QrCodeNumber);
                }
            }
        }

        /* This function couples up the item ID with the QRCode */
        public bool CoupleQrCode(string itemId, string qrNumber)
       {
          /*  Random generator = new Random();
            String r = generator.Next(0, 1000000).ToString("D6");       //create a random 6 digit string 
            */
           // itemId = qrNumber;        //THIS SHOULD BE FIXED ONCE THE DB IS DONE
           /*   bool successCode = false;                   //boolean to check if the coupling was successful, used as return 

            if (itemId != null || qrNumber != null)
            {
                successCode = true;
            }
            else
            {
                return successCode;         //should return false if coupling never occurs 
            }
            return successCode;*/
            return true;
        }


        /* This function is used to return the structure of the QRCode page  */
        public IActionResult Index()
        {
            return View();
        }

        /* This function is used to generate and return the QRcode for an item*/
        [HttpPost]
        public IActionResult Index(string itemId)
        {
            var qrCodeModel = new QrCodeModel();
            GenerateQrCode(itemId, qrCodeModel);
            return View();
        }
    }
}
