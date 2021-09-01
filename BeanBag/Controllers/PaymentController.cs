using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace BeanBag.Controllers
{
    public class PaymentController : Controller
    {
        public IActionResult Billing()
        {
            return View();
        }
    }
    
}