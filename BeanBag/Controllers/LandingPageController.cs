using System;
using System.Diagnostics;
using System.Linq;
using BeanBag.Database;
using BeanBag.Models;
using BeanBag.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using X.PagedList;

namespace BeanBag.Controllers
{
    // This is the Landing page controller.
    [AllowAnonymous]
    public class LandingPageController : Controller
    {
        private readonly TenantService _tenantService;
        private readonly TenantDbContext _tenantDb;

        public LandingPageController(TenantService service, TenantDbContext tenantDb)
        {
            _tenantService = service;
            _tenantDb = tenantDb;
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
        // Allows user to create a new tenant
        public IActionResult CreateTenant(string tenantName, string tenantTheme)
        {
            if (tenantName == null)
            {
                return RedirectToAction("Index");
            }
            else
            {
               _tenantService.CreateNewTenant(tenantName);
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        // Allows user to select tenant before sign in or sign up
        public IActionResult SelectTenant(string tenant)
        {
            if (tenant == null)
            {
                return RedirectToAction("Index");
            }
            else
            {
                _tenantService.SetCurrentTenant(tenant);
            }

            return NoContent();
        }
    }
}
