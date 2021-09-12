using BeanBag.Models;
using BeanBag.Models.Helper_Models;
using BeanBag.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.CognitiveServices.Vision.CustomVision.Training.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using X.PagedList;

namespace BeanBag.Controllers
{
    /* This controller is used to send and retrieve data to the dashboard
       view using AI Model and Blob Service functions. */
    public class AiModelController : Controller
    {
        // Global variables needed for calling the service classes.
        private readonly IAIService _aIService;
        private readonly IBlobStorageService _blobService;
    
        // Constructor.
        public AiModelController(IAIService aIService, IBlobStorageService blobService)
        {
            _aIService = aIService;
            _blobService = blobService;
        }

        /* This function adds a page parameter, a current sort order parameter, and a current filter
         parameter to the method signature and returns a view model with pagination to return an AI model list. */
        public IActionResult Index(string sortOrder, string currentFilter, string searchString, int? page,
            DateTime from, DateTime to)
        {
            if(User.Identity is {IsAuthenticated: true})
            {
                
             //A ViewBag property provides the view with the current sort order, because this must be included in 
             //  the paging links in order to keep the sort order the same while paging
            ViewBag.CurrentSort = sortOrder;
            ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            List<AIModel> modelList;

            //ViewBag.CurrentFilter, provides the view with the current filter string.
            //the search string is changed when a value is entered in the text box and the submit
            //button is pressed. In that case, the searchString parameter is not null.
            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewBag.CurrentFilter = searchString;


            var model = from s in _aIService.getAllModels() select s;
            
                //Search and match data, if search string is not null or empty
                if (!String.IsNullOrEmpty(searchString))
                {
                    model = model.Where(s => s.name.Contains(searchString));
                }
                switch (sortOrder)
                {
                    case "name_desc":
                        modelList = model.OrderByDescending(s => s.name).ToList();
                        break;
                 
                    default:
                        modelList = model.OrderBy(s => s.name).ToList();
                        break;
                }
                
                //TO DO: Date sorting --- need to add sorting once date created to DB 
                /*     if (sortOrder == "date")
                {
                    modelList =( model.Where(t => t.createdDate > from && t.createdDate < to)).ToList();

                }*/
                
            //indicates the size of list
            int pageSize = 5;
            
            //set page to one is there is no value, ??  is called the null-coalescing operator.
            int pageNumber = (page ?? 1);
            
            //Initialise data and models to be returned to the view.
            AIModel mod = new AIModel();
            Pagination viewModel = new Pagination();
            IPagedList<AIModel> pagedList = modelList.ToPagedList(pageNumber, pageSize);
            
            viewModel.AIModel = mod;
            viewModel.PagedListModels = pagedList;
            @ViewBag.totalModels = _aIService.getAllModels().Count;
            

            return View(viewModel);
            }
            else
            {
                return LocalRedirect("/");
            }
        }
        
         /* This function adds a page parameter, a current sort order parameter, and a current filter parameter
          to the method signature and returns a view model with pagination to return an AI model version list. */
        public IActionResult ModelVersions(Guid projectId, string sortOrder, string currentFilter, 
            string searchString, int? page,DateTime from, DateTime to)
        {
            if(User.Identity is {IsAuthenticated: true})
            {
                
             //A ViewBag property provides the view with the current sort order, because this must be included in 
             //  the paging links in order to keep the sort order the same while paging
            ViewBag.CurrentSort = sortOrder;
            ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            List<AIModelVersions> modelList;

            
         
            //ViewBag.CurrentFilter, provides the view with the current filter string.
            //the search string is changed when a value is entered in the text box and the submit button is
            //pressed. In that case, the searchString parameter is not null.
            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewBag.CurrentFilter = searchString;

            var model = from s in _aIService.getProjectIterations(projectId) 
                select s;
            
                //Search and match data, if search string is not null or empty
                if (!String.IsNullOrEmpty(searchString))
                {
                    model = model.Where(s => s.Name.Contains(searchString));
                }
                switch (sortOrder)
                {
                    case "name_desc":
                        modelList = model.OrderByDescending(s => s.Name).ToList();
                        break;
                 
                    default:
                        modelList = model.OrderBy(s => s.Name).ToList();
                        break;
                }

                //TO DO: Date sorting --- need to add date created to DB 
                /*     if (sortOrder == "date")
                {
                    modelList =( model.Where(t => t.createdDate > from && t.createdDate < to)).ToList();
                }*/
                
            //indicates the size of list
            int pageSize = 5;
            
            //set page to one is there is no value, ??  is called the null-coalescing operator.
            int pageNumber = (page ?? 1);
            
            //Setting models to be returned to the view
            AIModelVersions mod = new AIModelVersions();
            Pagination viewModel = new Pagination();
            IPagedList<AIModelVersions> pagedList = modelList.ToPagedList(pageNumber, pageSize);
            
            viewModel.AIModelVersions = mod;
            viewModel.PagedListVersions = pagedList;
            @ViewBag.totalModels = _aIService.getProjectIterations(projectId).Count;
            ViewBag.projectId = projectId;

            if (_aIService.getModel(projectId).imageCount == _aIService.getImageCount(projectId))
                ViewBag.canTrainNewVersion = false;
            else
                ViewBag.canTrainNewVersion = true;

            ViewBag.recommendations = _aIService.AIModelRecommendations(projectId); ;
            ViewBag.modelTags = _aIService.getModelTags(projectId);

            //IList<Tag> tags = _aIService.getModelTags(projectId);
            //foreach(var t in tags)
            //    t.ima

            return View(viewModel);
            }
            else
            {
                return LocalRedirect("/");
            }
        }
        
        // This function is the post method used to create an AI Model instance using the create project AI Service.
        [HttpPost]
        public async Task<IActionResult> CreateModel(Pagination mods)
        {
            Guid id = await _aIService.createProject(mods.AIModel.name, mods.AIModel.description);

            return LocalRedirect("/AIModel/TestImages?projectId=" + id.ToString());
        }

        // This function returns the view along with the name of the model to the test image AI model page.
        public IActionResult TestImages(Guid projectId)
        {
            @ViewBag.ID = projectId;

            var model = _aIService.getModel(projectId);
            ViewBag.Name = model.name;
            ViewBag.Description = model.description;

            if (model.imageCount == 0)
                ViewBag.newProject = true;
            else
                ViewBag.newProject = false;

            return View();
        }

        // This function allows the user to upload images to train a new AI Model.
        [HttpPost]
        public async Task<IActionResult> UploadTestImages([FromForm(Name = "files")] IFormFileCollection files,
            [FromForm(Name ="projectId")] Guid projectId, [FromForm(Name ="tags")] string[] tags,
            [FromForm(Name = "LastTestImages")] string lastTestImages)
        {
            var model = _aIService.getModel(projectId);

            if(files.Count < 5)
                return LocalRedirect("/AIModel/TestImages?projectId=" + projectId.ToString());

            if(files.Count > 50)
                return LocalRedirect("/AIModel/TestImages?projectId=" + projectId.ToString());


            if (files.Count == 0)
                return LocalRedirect("/AIModel/TestImages?projectId=" + projectId.ToString());

            //Checking if each tag is not empty or not an empty string
            foreach (var tag in tags)
            {
                if (tag.Equals("") || tag.Equals(" "))
                {
                    return Ok("Tag entry invalid");
                }
            }
            
            List<string> imageUrls = await _blobService.uploadTestImages(files, projectId.ToString());
            _aIService.uploadTestImages(imageUrls, tags, projectId);

            if (lastTestImages != null)
                return LocalRedirect("/AIModel/ModelVersions?projectId=" + projectId.ToString());
            else
                return LocalRedirect("/AIModel/TestImages?projectId=" + projectId.ToString());
        }

        // This function allows the user to train the AI model they had created by calling TrainModel AI Model service.
        public IActionResult TrainModel(Guid projectId)
        {
            _aIService.trainModel(projectId);
            return LocalRedirect("/AIModel/ModelVersions?projectId=" + projectId.ToString());
        }

        // This function allows the user to publish an iteration by calling the PublishIteration AI Model service.
        public IActionResult PublishIteration(Guid projectId, Guid iterationId)
        {
            _aIService.publishIteration(projectId, iterationId);
            return LocalRedirect("/AIModel/ModelVersions?projectId=" + projectId.ToString());
        }

        // This function allows the user to publish an iteration by calling the publishIteration AI Model service.
        public IActionResult UnpublishIteration(Guid projectId, Guid iterationId)
        {
            _aIService.unpublishIteration(projectId, iterationId);
            return LocalRedirect("/AIModel/ModelVersions?projectId=" + projectId.ToString());
        }

        // This function allows the user to edit a model by calling the EditModel AI Model service.
        [HttpPost]
        public IActionResult EditAIModelPost(Guid projectId, string projectName, string description)
        {
            _aIService.editProject(projectId, projectName, description);
            return LocalRedirect("/AIModel");
        }

        public IActionResult EditAIModel(Guid Id)
        {
            if(User.Identity is { IsAuthenticated: true})
            {
                var model = _aIService.getModel(Id);
                if (model == null)
                    return NotFound();

                return View(model);
            }
            else
            {
                return LocalRedirect("/");
            }
        }

        // This function allows the user to delete a model by calling the DeleteModel AI Model service.
        [HttpPost]
        public IActionResult DeleteAIModelPost(Guid projectId)
        {
            _aIService.deleteProject(projectId);
            return LocalRedirect("/AIModel");
        }

        public IActionResult DeleteAIModel(Guid projectId)
        {
            if (User.Identity is { IsAuthenticated: true })
            {
                var model = _aIService.getModel(projectId);
                if (model == null)
                    return NotFound();

                return View(model);
            }
            else
            {
                return LocalRedirect("/");
            }
        }
        
        // This function allows the user to delete a model version by calling the DeleteVersion AI Model service.
        [HttpPost]
        public IActionResult DeleteVersionPost(Guid projectId, Guid iterationId)
        {
            _aIService.deleteIteration(iterationId);
            return LocalRedirect("/AIModel/ModelVersions?projectId=" + projectId.ToString());
        }

        public IActionResult DeleteVersion(Guid Id)
        {
            if(User.Identity is { IsAuthenticated: true})
            {
                var version = _aIService.getIteration(Id);
                if (version == null)
                    return NotFound();

                return View(version);
            }
            else
            {
                return LocalRedirect("/");
            }
        }

        // This function returns all of the performance metrics for the AI Model version
        public IActionResult ModelVersionPerformace(Guid projectId, Guid iterationId)
        {
            string iterationMetrics = "";
            string tagPerformance = "";

            IterationPerformance modelPerformace = _aIService.getModelVersionPerformance(projectId, iterationId);
            List<AIModelVersionTagPerformance> tagsPerformace = _aIService.getPerformancePerTags(projectId, iterationId, modelPerformace);

            foreach(var tag in tagsPerformace)
            {
                tagPerformance += tag.tagName + '-' + tag.precision*100 + "%-" + tag.recall*100 + "%-" + tag.averagePrecision*100 + "%-" + tag.imageCount + "\n";
            }

            iterationMetrics = modelPerformace.Precision*100 + "%-" + modelPerformace.Recall*100 + "%-" + modelPerformace.AveragePrecision*100 + "%\n";

            return Ok(iterationMetrics + "\n" + tagPerformance);
        }

        public IActionResult EditVersion(Guid Id)
        {
            if (User.Identity is { IsAuthenticated: true })
            {
                var version = _aIService.getIteration(Id);
                if (version == null)
                    return NotFound();

                return View(version);
            }
            else
            {
                return LocalRedirect("/");
            }
        }

        public IActionResult EditVersionPost(Guid projectId, Guid Id, string description)
        {
            _aIService.EditIteration(Id, description);
            return LocalRedirect("/AIModel/ModelVersions?projectId=" + projectId.ToString());
        }

    }
}
