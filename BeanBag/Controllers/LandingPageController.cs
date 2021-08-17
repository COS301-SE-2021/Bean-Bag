using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BeanBag.Controllers
{
    // This is the Landing page controller.
    [AllowAnonymous]
    public class LandingPageController : Controller
    {
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

    }
}
