using System;
using System.ComponentModel.Design;
using BeanBag.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Web;

namespace BeanBag.Controllers
{
    public class AccountController : Controller
    {
        private readonly TenantService _tenantService;

        public AccountController(TenantService tenantService)
        {
            _tenantService = tenantService;
        }
        
        [AllowAnonymous]
        public IActionResult Index()
        {
            if (User.GetObjectId() == null)
            {
                throw new Exception("User id is null");
            }

            if (_tenantService.SearchUser(User.GetObjectId()))
            {
                return RedirectToAction("Index", "Home");
            }
            
            return View();
        }
        
        [HttpPost]
        // Allows user to create a new tenant
        public IActionResult CreateTenant(string tenantName)
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
        // Allows user to select a tenant 
        public IActionResult SelectTenant(string tenant)
        {
            if (tenant == null)
            {
                return RedirectToAction("Index");
            }

            //Check if user is new
            var userId = User.GetObjectId();
            
            var currentTenantName = tenant;

            var currentTenantId = _tenantService.GetTenantId(currentTenantName);

            if (_tenantService.SearchUser(userId) == false)
            {
                //User is new - add user to database
                //Verify tenant
                if (_tenantService.SearchTenant(currentTenantId))
                {
                    //Verified
                    _tenantService.SignUserUp(userId, currentTenantId);
                }
                else
                {
                    throw new Exception("Tenant does not exist");
                }
            }
            

            return RedirectToAction("Index", "Home");
        }

    }
}