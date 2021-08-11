using System;
using System.Diagnostics;
using BeanBag.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BeanBag.Controllers
{
    // This is the Landing page controller.
    [AllowAnonymous]
    public class LandingPageController : Controller
    {
        private readonly TenantService _tenantService;

        public LandingPageController(TenantService service)
        {
            _tenantService = service;
        }
        
        // This function sends a response to the LandingPage Index page.
        public IActionResult Index()
        {
            return View();
        }
        
        [HttpGet]
        // This function redirects to Landing Page when signing out.
        public IActionResult SignOut(string page)
        {
            return RedirectToAction("Index", "LandingPage");
        }

        [HttpPost]
        public IActionResult CreateTenant(string tenantName, string tenantTheme)
        {
            if (tenantName == null)
            {
                return RedirectToAction("Index");
            }
            else
            {
               // _tenantService.CreateNewTenant(tenantName, tenantTheme);
            }

            return RedirectToAction("Index");
        }
    }
}
