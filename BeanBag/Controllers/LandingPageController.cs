using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BeanBag.Controllers
{
    /* This class is responsible for the data returned to the landing page */
    [AllowAnonymous]
    public class LandingPageController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        
        [HttpGet]
        /* Redirects to Landing Page when signing out */
        public IActionResult SignOut(string page)
        {
            return RedirectToAction("Index", "LandingPage");
        }
    }
}
