using Microsoft.AspNetCore.Mvc;

namespace BeanBag.Controllers
{
    /*
     * This class is responsible for the data returned to the landing page 
     */
    public class LandingPageController : Controller
    {
        /*
         * This function returns the structure of the Landing Page 
         */
        public IActionResult Index()
        {
            return View();
        }
    }
}
