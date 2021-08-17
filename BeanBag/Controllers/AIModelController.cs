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

            return LocalRedirect("/AIModel/TestImages?projectId=" + id.ToString());
        }

        public IActionResult TestImages(Guid projectId)
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
                return LocalRedirect("/AIModel/ModelVersions?projectId=" + projectId.ToString());
            else
                return LocalRedirect("/AIModel/TestImages?projectId=" + projectId.ToString());

        }

        public IActionResult ModelVersions(Guid projectId)
        {
            List<AIModelVersions> modelversions = aIService.getProjectIterations(projectId);         
            ViewBag.projectId = projectId;
            return View(modelversions);
        }

        public IActionResult TrainModel(Guid projectId)
        {
            aIService.trainModel(projectId);
            return LocalRedirect("/AIModel/ModelVersions?projectId=" + projectId.ToString());
        }

        public IActionResult PublishIteration(Guid projectId, Guid iterationId)
        {
            aIService.publishIteration(projectId, iterationId);
            return LocalRedirect("/AIModel/ModelVersions?projectId=" + projectId.ToString());
        }

        public IActionResult Unpublishiteration(Guid projectId, Guid iterationId)
        {
            aIService.unpublishIteration(projectId, iterationId);
            return LocalRedirect("/AIModel/ModelVersions?projectId=" + projectId.ToString());
        }

        public IActionResult EditModel()
        {
            throw new NotImplementedException();
        }

        public IActionResult DeleteModel()
        {
            throw new NotImplementedException();
        }

        public IActionResult Edit()
        {
            throw new NotImplementedException();
        }
    }
}
