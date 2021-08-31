using Microsoft.AspNetCore.Mvc;

namespace BeanBag.Controllers
{
    public class TenantSettingsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}