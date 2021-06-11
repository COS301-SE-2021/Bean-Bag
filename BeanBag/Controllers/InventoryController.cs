using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BeanBag.Controllers
{
    // This class is used to handle any user interaction regarding an inventory
    public class InventoryController : Controller
    {
        // This is the default view to view all of the inventories associated with a user
        public IActionResult Index()
        {
            return View();
        }
    }
}
