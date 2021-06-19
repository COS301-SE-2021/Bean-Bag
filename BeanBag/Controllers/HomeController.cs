using Microsoft.AspNetCore.Mvc;
namespace BeanBag.Controllers
{
    // This is the Home page controller.
    public class HomeController : Controller
    {
        // This function sends a response to the Home Index page.
        public IActionResult Index()
        {
            return View();
        }
    }
}
