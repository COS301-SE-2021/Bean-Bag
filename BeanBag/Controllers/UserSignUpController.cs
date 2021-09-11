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
            
            //Get tenant id
            var tenantId = "";
            
            //Verify the entered code
            if (_tenantService.VerifyCode(tenantId,code))
            {
                return RedirectToAction("SignIn", "Account");
            }

            return RedirectToAction("Index");
        }

    }
}