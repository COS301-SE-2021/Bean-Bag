using Microsoft.AspNetCore.Mvc;
namespace BeanBag.Controllers
{
    /*
     * This class is responsible for data returned to the Home Page 
     */
    public class HomeController : Controller
    {
        /*
        * This function returns the page structure for the items page 
        */
        public IActionResult Index()
        {
            return View();
        }
    }
}
