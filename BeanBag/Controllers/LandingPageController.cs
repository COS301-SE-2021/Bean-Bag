using Microsoft.AspNetCore.Mvc;

namespace BeanBag.Controllers
{
    /* This class is responsible for the data returned to the landing page */
    public class LandingPageController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
