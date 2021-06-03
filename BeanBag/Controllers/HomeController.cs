using BeanBag.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace BeanBag.Controllers
{
    /*
     * This class is responsible for data returned to the Home Page 
     */
    public class HomeController : Controller
    {
        /*
        * This function returns the page structure for the items page 
        */
        public IActionResult Index()
        {
            return View();
        }
    }
}
