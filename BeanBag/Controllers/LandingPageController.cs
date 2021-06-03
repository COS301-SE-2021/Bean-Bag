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
//Added imports


//controllers in mvc
//controllers help the model and view interact
//take stuff from the model and output it to view
//logically, QRCode is our model and it is outputted as a view
namespace BeanBag.Controllers
{
    public class LandingPageController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(string inputText)    //Index page but specified with the string input
        {
            using (MemoryStream ms = new MemoryStream())        //memory stream helps write from and to the 'memory'
            {
                QRCodeGenerator qRCodeGenerator = new QRCodeGenerator();    //newQRCode generator Object instance created from package
                QRCodeData qRCodeData = qRCodeGenerator.CreateQrCode(inputText, QRCodeGenerator.ECCLevel.Q);
                //Data and input text is used to generate the QR Code's actual 'DATA' 
                QRCode qRCode = new QRCode(qRCodeData); //make a new instance of the QR code compiled data of text and string input
                using (Bitmap bitmap= qRCode.GetGraphic(20)) 
                {
                    bitmap.Save(ms, ImageFormat.Png);   //bitmap maps our string and data to image bits which are then 'drawn'
                    ViewBag.QRCode = "data:image/png;base64," + Convert.ToBase64String(ms.ToArray());  
                    //viewbag is connected to front end, and we convert the bitmap back to a value our front end can comprehend, an array of bits?
                }


            }
            return View();  
            //view is returned on the front end for the entire instance of the project 
            //The view in this case is the QR Code
            //in future the QR code should display a URL which is a simple web page for the item we just scanned.
            //we could formulate the URL from the info retrieved from the database
        }

        public IActionResult Signup()
        {
            return View();
        }
        public IActionResult Privacy()
        {
            return View();
        }
    }
}
