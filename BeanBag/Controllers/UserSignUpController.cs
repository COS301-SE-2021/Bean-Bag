using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BeanBag.Controllers
{
    public class UserSignUpController : Controller
    {

        [AllowAnonymous]
        public IActionResult Index()
        {
            return View();
        }

    }
}