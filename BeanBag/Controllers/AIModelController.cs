using BeanBag.Database;
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
        //private readonly DBContext db;

        public AIModelController(IAIService _ai, IBlobStorageService _blob)
        {
            aIService = _ai;
            blobService = _blob;
        }

        public IActionResult Index()
        {
            List<AIModel> models = aIService.getAllModels();
            return View(models);
        }

        [HttpGet]
        public IActionResult CreateModel()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateModel(string modelName)
        {
            Guid id = await aIService.createProject(modelName);

            return LocalRedirect("/AIModel/TestImages?modelId=" + id.ToString());
        }

        public IActionResult TestImages(Guid modelId)
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> UploadTestImages([FromForm(Name = "files")] IFormFileCollection files, [FromForm(Name ="projectId")] Guid projectId, [FromForm(Name ="tags")] string[] tags, [FromForm(Name = "LastTestImages")] string lastTestImages)
        {
            //Checking if images are more than 5
            if(files.Count < 5)
            {
                //Change this erro handling
                return Ok("Image count less than 5");
            }
            //Checking if each tag is not empty or not an empty string
            foreach(var tag in tags)
            {
                if (tag.Equals("") || tag.Equals(" "))
                {
                    return Ok("Tag entry invalid");
                }
            }
            List<string> imageUrls = await blobService.uploadTestImages(files, projectId.ToString());

            aIService.uploadTestImages(imageUrls, tags, projectId);

            if (lastTestImages != null)
                return LocalRedirect("/AIModel/ModelVersions?modelId=" + projectId.ToString());
            else
                return LocalRedirect("/AIModel/TestImages?modelId=" + projectId.ToString());

        }

        public async Task<IActionResult> ModelVersions(Guid modelId)
        {
            List<AIModelVersions> modelversions = await aIService.getIterations(modelId);         
            ViewBag.ModelId = modelId;
            return View(modelversions);
        }

        public IActionResult TrainModel(Guid modelId)
        {
            aIService.trainModel(modelId);
            return LocalRedirect("/AIModel/ModelVersions?modelId=" + modelId.ToString());
        }

        public IActionResult PublishIteration(Guid projectId, Guid iterationId)
        {
            aIService.publishIteration(projectId, iterationId);
            return LocalRedirect("/AIModel/ModelVersions?modelId=" + projectId.ToString());
        }

        public IActionResult Unpublishiteration(Guid projectId, Guid iterationId)
        {
            aIService.unpublishIteration(projectId, iterationId);
            return LocalRedirect("/AIModel/ModelVersions?modelId=" + projectId.ToString());
        }
    }
}
