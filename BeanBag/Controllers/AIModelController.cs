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
                    model = model.Where(s => s.projectName.Contains(searchString));
                }
                switch (sortOrder)
                {
                    case "name_desc":
                        modelList = model.OrderByDescending(s => s.projectName).ToList();
                        break;
                 
                    default:
                        modelList = model.OrderBy(s => s.projectName).ToList();
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
                    model = model.Where(s => s.iterationName.Contains(searchString));
                }
                switch (sortOrder)
                {
                    case "name_desc":
                        modelList = model.OrderByDescending(s => s.iterationName).ToList();
                        break;
                 
                    default:
                        modelList = model.OrderBy(s => s.iterationName).ToList();
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
            Guid id = await _aIService.createProject(mods.AIModel.projectName);

            return LocalRedirect("/AIModel/TestImages?projectId=" + id.ToString());
        }

        // This function returns the view along with the name of the model to the test image AI model page.
        public IActionResult TestImages(Guid projectId)
        {
            @ViewBag.ID = projectId;
        
            var mods = _aIService.getAllModels();
            foreach (var t in mods.Where(t => t.projectId.Equals(projectId)))
            {
                @ViewBag.Name = t.projectName ;
            }
            return View();
        }

        // This function allows the user to upload images to train a new AI Model.
        [HttpPost]
        public async Task<IActionResult> UploadTestImages([FromForm(Name = "files")] IFormFileCollection files,
            [FromForm(Name ="projectId")] Guid projectId, [FromForm(Name ="tags")] string[] tags,
            [FromForm(Name = "LastTestImages")] string lastTestImages)
        {
            //Checking if images are more than 5
            if(files.Count < 5)
            {
                //Change this error handling
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
        public IActionResult EditModel(Guid projectId, string projectName, string description)
        {
            _aIService.editProject(projectId, projectName, description);
            return LocalRedirect("/AIModel");
        }

        // This function allows the user to delete a model by calling the DeleteModel AI Model service.
        public IActionResult DeleteModel(Guid projectId)
        {
            _aIService.deleteProject(projectId);
            return LocalRedirect("/AIModel");
        }

        // This function allows the user to edit a model version by calling the EditVersion AI Model service.
        public IActionResult EditVersion()
        {
            throw new NotImplementedException();
        }
        
        // This function allows the user to delete a model version by calling the DeleteVersion AI Model service.
        public IActionResult DeleteVersion(Guid projectId, Guid iterationId)
        {
            _aIService.deleteIteration(iterationId);
            return LocalRedirect("/AIModel/ModelVersions?projectId=" + projectId.ToString());
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

    }
}
