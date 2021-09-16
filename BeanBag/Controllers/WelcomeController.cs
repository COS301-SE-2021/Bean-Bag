using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using BeanBag.Services;
using Microsoft.Identity.Web;

namespace BeanBag.Controllers
{
    public class WelcomeController : Controller
    {

        private readonly ITenantService _tenantService;
        public WelcomeController(ITenantService tenantService)
        {
            _tenantService = tenantService;
        }

        [AllowAnonymous]
        public IActionResult Index()
        {
            return View();
        }

        
        [HttpPost]
        public IActionResult VerifyCode(string code)
        {
            if (code == null)
            {
                return BadRequest();
            }

            //Verify the entered code
            if (_tenantService.VerifyCode(code))
            {
                //Code is verified, sign user up under tenant 
                var tenant = _tenantService.GetInvitationTenant(code);

                if (tenant == null)
                {
                    return BadRequest();
                }

                if (_tenantService.SignUserUp(User.GetObjectId(), tenant.TenantId, User.GetDisplayName()))
                {
                    return RedirectToAction("Index", "Home");
                }

                return RedirectToAction("Index");
            }

            return RedirectToAction("Index");
        }

    }
}