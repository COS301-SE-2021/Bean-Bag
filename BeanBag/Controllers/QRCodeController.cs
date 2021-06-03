﻿using BeanBag.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using QRCoder;
namespace BeanBag.Controllers
{
    /*
     * This class is the controller for the QRCode, 
     */

    public class QRCodeController : Controller
    {

        /*
         * This function is used to return the structure of the QRCode page 
         */
        public IActionResult Index()
        {
            return View();
        }

        /*
         * This function is used to ...
         *
         */

        [HttpPost]
        public IActionResult Index(string inputText)    
        {
            /*
             * Dummy -- Mocking out backend to test [mocking recognised data that will come from AI model function]
             */
            string itemName = "Item: Table\n";
            string itemCondition = "Condition: Good\n";
            string color = "Color: Black\n";


            inputText = itemName + itemCondition + color;

            //memory stream helps write from and to the memory
            using (MemoryStream ms = new MemoryStream())        
            {
                QRCodeGenerator qRCodeGenerator = new QRCodeGenerator();    
                QRCodeData qRCodeData = qRCodeGenerator.CreateQrCode(inputText, QRCodeGenerator.ECCLevel.Q);

                //Data and input text is used to generate the QR Code's actual 'DATA' 
                QRCode qRCode = new QRCode(qRCodeData);

                //make a new instance of the QR code compiled data of text and string input
                using (Bitmap bitmap = qRCode.GetGraphic(20))
                {
                    //bitmap maps our string and data to image bits which are then 'drawn'
                    bitmap.Save(ms, ImageFormat.Png);   
                    ViewBag.QRCode = "data:image/png;base64," + Convert.ToBase64String(ms.ToArray());
                    //viewbag is connected to front end, and we convert the bitmap back to a value our front end can comprehend, an array of bits?
                }

            }
            //view is returned on the front end for the entire instance of the project 
            return View();  
        }
    }
}
