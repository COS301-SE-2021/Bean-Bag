using BeanBag.Models;
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
                QRCodeData qRCodeData = qRCodeGenerator.CreateQrCode("urlfromthedatabase", QRCodeGenerator.ECCLevel.Q);
                //Data and input text is used to generate the QR Code's actual 'DATA' 
                QRCode qRCode = new QRCode(qRCodeData); //make a new instance of the QR code compiled data of text and string input
                using (Bitmap bitmap= qRCode.GetGraphic(20)) 
                {
                    bitmap.Save(ms, ImageFormat.Png);   //bitmap maps our string and data to image bits which are then 'drawn'
                    ViewBag.QRCode = "data:image/png;base64," + Convert.ToBase64String(ms.ToArray());  
                    //viewbag is connected to front end, and we convert the bitmap back to a value our front end can comprehend, an array of bits?
                }


            }
            return View();  //view is returned on the front end for the entire instance of the project ;)
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
