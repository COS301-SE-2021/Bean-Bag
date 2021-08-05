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

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(string modelName)
        {
            Guid id = await aIService.createProject(modelName);

            return Ok(id);
        }
    }
}
