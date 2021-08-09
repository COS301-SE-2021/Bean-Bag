using System;
using System.Drawing.Printing;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace BeanBag.Controllers
{
    public class AppearanceController : Controller
    {
        // This function sends a response to the Home Index page.
        public IActionResult Index()
        {
            return View();
        }



        
        [HttpPost]
        public IActionResult ChangeThemeColour()
        {
            //Instantiated a variable that will hold the selected theme 
            string theme = Request.Form["theme"];
            Console.WriteLine("the value is "+theme);
            //Pass the theme into a function that will save it into the DB
            
            return RedirectToAction("Index");
        }
    }
}