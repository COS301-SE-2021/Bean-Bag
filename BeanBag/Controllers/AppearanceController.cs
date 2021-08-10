using System;
using BeanBag.Database;
using BeanBag.Models;
using BeanBag.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Web;

namespace BeanBag.Controllers
{
    public class AppearanceController : Controller
    {
        private readonly TenantService _tenantService;

        public AppearanceController(TenantService tenantService)
        {
            _tenantService = tenantService;
        }

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
            
            //Pass the theme into a function that will save it into the DB
            _tenantService.SetTenantTheme(User.GetObjectId(), theme);

            return RedirectToAction("Index", "Home");
        }
    }
}