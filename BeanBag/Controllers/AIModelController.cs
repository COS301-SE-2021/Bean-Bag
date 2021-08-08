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
        private readonly IBlobStorageService blobService;

        public AIModelController(IAIService _ai, IBlobStorageService _blob)
        {
            aIService = _ai;
            blobService = _blob;
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

            return LocalRedirect("/AIModel/TestImages?modelId=" + id.ToString());
        }

        public IActionResult TestImages(string modelId)
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> UploadTestImages([FromForm(Name = "files")] IFormFileCollection files, [FromForm(Name ="projectId")] Guid projectId, [FromForm(Name ="tags")] string[] tags)
        {
            List<string> imageUrls = await blobService.uploadTestImages(files, projectId.ToString());

            aIService.uploadTestImages(imageUrls, tags, projectId);

            return Ok(":)");
        }
    }
}
