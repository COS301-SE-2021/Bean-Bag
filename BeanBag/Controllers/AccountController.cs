using BeanBag.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BeanBag.Controllers
{
    public class AccountController : Controller
    {
        private readonly TenantService _tenantService;
        
        [AllowAnonymous]
        public IActionResult Index()
        {
            return View();
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

    }
}