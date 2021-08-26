using Microsoft.AspNetCore.Mvc;

namespace BeanBag.Controllers
{
    public class PaymentController : Controller
    {
        // GET
        public IActionResult Index()
        {
            return View();
        }
    }
}