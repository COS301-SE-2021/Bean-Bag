using BeanBag.Models;
using BeanBag.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BeanBag.Controllers
{
    public class AIModelController : Controller
    {
        private readonly IAIService aIService;

        public AIModelController(IAIService _ai)
        {
            aIService = _ai;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult newAIModel(CreateNewAIModel newModel)
        {
            // Create the project with it's new name
            // Upload images along with tags needed with images (image count > 5). List<Tags>, IFormCollection

            return Ok();
        }
    }
}
