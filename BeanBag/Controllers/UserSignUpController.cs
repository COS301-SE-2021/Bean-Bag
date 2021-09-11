using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using BeanBag.Services;

namespace BeanBag.Controllers
{
    public class UserSignUpController : Controller
    {

        private readonly ITenantService _tenantService;
        public UserSignUpController(ITenantService tenantService)
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
                return RedirectToAction("Index");
            }

            return RedirectToAction("Index");
        }

    }
}