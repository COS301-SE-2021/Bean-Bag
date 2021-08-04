using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace BeanBag.Controllers
{
    public class AppearanceController : Controller
    {
        // This function sends a response to the Home Index page.
        public IActionResult Index()
        {
            return View();
        }
    }
}