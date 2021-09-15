using BeanBag.Models;
using BeanBag.Models.Helper_Models;
using BeanBag.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.CognitiveServices.Vision.CustomVision.Training.Models;
using Microsoft.Identity.Web;
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
        private readonly ITenantService _tenantService;
    
        // Constructor.
        public AiModelController(IAIService aIService, IBlobStorageService blobService, ITenantService tenantService)
        {
            _aIService = aIService;
            _blobService = blobService;
            _tenantService = tenantService;
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
            
                viewModel.AiModel = mod;
                viewModel.PagedListModels = pagedList;
                @ViewBag.totalModels = _aIService.getAllModels().Count;

                //Checking to see if the tenant is allowed to create more AI Models
                Tenant tenant = _tenantService.GetCurrentTenant(User.GetObjectId());

                List<AIModel> models = _aIService.getAllModels();

                if(tenant.TenantSubscription == "Free")
                {
                    if (models.Count == 0)
                        ViewBag.createNewModels = true;
                    else
                        ViewBag.createNewModels = false;
                }
                else if(tenant.TenantSubscription == "Standard")
                {
                    if (models.Count < 3)
                        ViewBag.createNewModels = true;
                    else
                        ViewBag.createNewModels = false;
                }
                else if(tenant.TenantSubscription == "Premium")
                {
                    if (models.Count < 20)
                        ViewBag.createNewModels = true;
                    else
                        ViewBag.createNewModels = false;
                }


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
            
          
                viewModel.PagedListVersions = pagedList;
                @ViewBag.totalModels = _aIService.getProjectIterations(projectId).Count;
                ViewBag.projectId = projectId;

                ViewBag.recommendations = _aIService.AIModelRecommendations(projectId); ;
                ViewBag.modelTags = _aIService.getModelTags(projectId);

                //IList<Tag> tags = _aIService.getModelTags(projectId);
                //foreach(var t in tags)
                //    t.ima


                int? imageCount = _aIService.getImageCount(projectId);
                ViewBag.modelTraining = false;
                Tenant tenant = _tenantService.GetCurrentTenant(User.GetObjectId());
                List<AIModelVersions> versions = _aIService.getProjectIterations(projectId);

                // If their are any iterations in training then the admin cannot train a new model version
                foreach(var v in versions)
                {
                    if(v.status == "Training")
                    {
                        ViewBag.modelTraining = true;
                        return View(viewModel);
                    }
                }

                //Custom vision has a cap of 20 iterations per model
                if(versions.Count >= 20)
                {
                    ViewBag.modelTraining = true;
                    return View(viewModel);
                }


                // Check tenant subscriptio to cap the amount of iterations allowed to be created
                if (tenant.TenantSubscription == "Free")
                {
                    if (versions.Count < 3)
                    {
                        if (_aIService.getModel(projectId).imageCount == imageCount)
                            ViewBag.canTrainNewVersion = false;
                        else
                            ViewBag.canTrainNewVersion = true;
                    }
                    else
                        ViewBag.createNewModels = false;
                }
                else if (tenant.TenantSubscription == "Standard")
                {
                    if (versions.Count < 10)
                    {
                        if (_aIService.getModel(projectId).imageCount == imageCount)
                            ViewBag.canTrainNewVersion = false;
                        else
                            ViewBag.canTrainNewVersion = true;
                    }
                    else
                        ViewBag.createNewModels = false;
                }
                else if (tenant.TenantSubscription == "Premium")
                {
                    if (_aIService.getModel(projectId).imageCount == imageCount)
                        ViewBag.canTrainNewVersion = false;
                    else
                        ViewBag.canTrainNewVersion = true;
                }

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
            Guid id = await _aIService.createProject(mods.AiModel.name, mods.AiModel.description);

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
            
           ViewBag.complainImages = "";
            var model = _aIService.getModel(projectId);

            if(files.Count < 5)
            {
                //ModelState.AddModelError("", "Need to upload more than 5 images");
                ViewBag.complainImages = "Need to upload more than 5 images";
                return LocalRedirect("/AIModel/TestImages?projectId=" + projectId.ToString());
            }
            else if(files.Count > 1000)
            {
                //ViewBag.complainImages = "Cannot upload more than 1000 images at a time";
                return LocalRedirect("/AIModel/TestImages?projectId=" + projectId.ToString());
            }
                
            //Checking if each tag is not empty or not an empty string
            foreach (var tag in tags)
            {
                if (tag.Equals("") || tag.Equals(" "))
                {
                    //ViewBag.complainImages = "Tag text field cannot be empty";
                    return LocalRedirect("/AIModel/TestImages?projectId=" + projectId.ToString());
                }
            }

            // Custom Vision cannot have more than 100 000 images.
            if(_aIService.getImageCount(projectId) + files.Count >= 100000)
            {
                //ViewBag.complainImages = "An AI model cannot have more than 100 000 images.";
                return LocalRedirect("/AIModel/ModelVersions?projectId=" + projectId.ToString());
            }

            //Custom vision cannot have more than 500 tags
            if(_aIService.getModelTags(projectId).Count + tags.Length >= 500)
            {
                //ViewBag.complainImages = "An AI model cannot have more than 500 tags";
                return LocalRedirect("/AIModel/ModelVersions?projectId=" + projectId.ToString());
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
            List<AIModelVersionTagPerformance> tagsPerformace = _aIService.getPerformancePerTags(projectId, iterationId);

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

        
        public IActionResult ViewPerformance(Guid Id)
        {
            if(User.Identity is { IsAuthenticated: true})
            {
                var iteration = _aIService.getIteration(Id);
                Guid projectId = iteration.projectId;

                List<AIModelVersionTagPerformance> tags =  _aIService.getPerformancePerTags(projectId, iteration.Id);

                ViewBag.iterationId = Id;
                ViewBag.projectId = projectId;

                return View(tags);
            }
            else
            {
                return LocalRedirect("/");
            }
        }

        [HttpGet]
        public IActionResult DeleteTag(Guid Id, Guid projectId, int imageCount, string Name)
        {
            if (User.Identity is { IsAuthenticated: true })
            {
                var tag = new ModelTag()
                {
                    Id = Id, 
                    name = Name, 
                    imageCount = imageCount, 
                    projectId = projectId
                };

                return View(tag);
            }
            else
            {
                return LocalRedirect("/");
            }
        }

        [HttpPost]
        public IActionResult DeleteTag(Guid Id, Guid projectId, int imageCount)
        {
            _aIService.deleteModelTag(Id, projectId, imageCount);

            return LocalRedirect("/AIModel/ModelVersions?projectId=" + projectId.ToString());
        }

    }
}
