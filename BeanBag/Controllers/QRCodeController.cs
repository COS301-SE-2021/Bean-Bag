﻿using BeanBag.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Drawing.Imaging;
using System.IO;
using QRCoder;

namespace BeanBag.Controllers
{
    // This is the QR Code controller.
    public class QrCodeController : Controller
    {
        private QrCodeModel _qrModel = new QrCodeModel();
        
        //---------------------------------- SERVICE CONTRACTS----------------------------------------- 

        /* This function generates a QRCode and gets the itemDetails as the parameter. Memory stream helps write from
         * and to the memory. Data and input text is used to generate the QR Code's actual 'DATA'. A new instance of
         * the QR code compiled data of text and string input is created.bitmap maps our string and data to image bits
         * which are then 'drawn', thereafter the generated QRCode variable is set.*/
        public bool GenerateQrCode(string itemId)
        {
            if (itemId == null)
            {
                throw new Exception("QRCode generation failed. ItemID input is null.");
            }

            else if (itemId.Length !=36) 
            {
                throw new Exception("QRCode generation failed. ItemID string length is invalid.");
            }
            
            // Query from db here
            // Throw exception if item not found
            // Throw new Exception("QRCode generation failed. ItemID not found in database.");
            const string itemName = "Item: Chair\n";
            const string itemInventory = "Inventory: Furniture Inventory\n";
            const string iType = "Type: Furniture\n";
            itemId = itemName + itemInventory + iType;

            var ms = new MemoryStream();
            var qRCodeGenerator = new QRCodeGenerator();
            var qRCodeData = qRCodeGenerator.CreateQrCode(itemId, QRCodeGenerator.ECCLevel.Q);
            var qRCode = new QRCode(qRCodeData);
            var bitmap = qRCode.GetGraphic(20);
            
            bitmap.Save(ms, ImageFormat.Png);
            ViewBag.QRCode = "data:image/png;base64," + Convert.ToBase64String(ms.ToArray());

            if (ViewBag.QRCode == null)
            {
                return false;
            }
            
            _qrModel.QrCodeNumber = qRCode.ToString();
             CoupleQrCode(itemId, _qrModel.QrCodeNumber);
            
            return true;
        }

        //This function couples up the item ID with the QRCode.
        public bool CoupleQrCode(string itemId, string qrNumber)
        {
            // Random generator = new Random();
            // Create a random 6 digit string 
            // String r = generator.Next(0, 1000000).ToString("D6");
            // itemId = qrNumber;        //THIS SHOULD BE FIXED ONCE THE DB IS DONE
            // Boolean to check if the coupling was successful, used as return 
            bool successCode = itemId != null || qrNumber != null;
            // Should return false if coupling never occurs 
            return successCode;
        }

        //---------------------------------- VIEW RESPONSE----------------------------------------- 

        // This function returns the QRCode Index page.
        public IActionResult Index()
        {
            return View();
        }

        // This function is used to generate and respond with the QR-code for an item to the View.
        [HttpPost]
        public IActionResult Index(string itemId)
        {
            _qrModel = new QrCodeModel();
            
            // Mock id until integrate.
            GenerateQrCode(new Guid().ToString());
            return View();
        }
    }
}
