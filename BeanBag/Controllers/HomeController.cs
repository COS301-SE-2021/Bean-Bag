using Microsoft.AspNetCore.Mvc;
namespace BeanBag.Controllers
{
    // The default home page controller 
    /* This class is responsible for data returned to the Home Page */
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
