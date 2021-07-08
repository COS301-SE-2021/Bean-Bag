using Microsoft.AspNetCore.Mvc;

namespace BeanBag.Controllers
{
    public class DashboardController : Controller
    {
        // GET
        public IActionResult Index()
        {
            return View();
        }
    }
}