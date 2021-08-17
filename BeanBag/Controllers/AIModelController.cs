using BeanBag.Models;
using BeanBag.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Identity.Web;
using X.PagedList;

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

   /*     public IActionResult Index()
        {
            List<AIModel> models = aIService.getAllModels();
            return View(models);
        }*/
        
             
         //This code adds a page parameter, a current sort order parameter, and a current filter parameter to the method signature
        public IActionResult Index(string sortOrder, string currentFilter, string searchString, int? page,DateTime from, DateTime to)
        {
            if(User.Identity is {IsAuthenticated: true})
            {
                
             //A ViewBag property provides the view with the current sort order, because this must be included in 
             //  the paging links in order to keep the sort order the same while paging
            ViewBag.CurrentSort = sortOrder;
            ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            List<AIModel> modelList;

            //ViewBag.CurrentFilter, provides the view with the current filter string.
            //he search string is changed when a value is entered in the text box and the submit button is pressed. In that case, the searchString parameter is not null.
            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewBag.CurrentFilter = searchString;


            var model = from s in aIService.getAllModels()
                select s;
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

                //Date sorting --- need to add date created to DB 
           /*     if (sortOrder == "date")
                {
                    modelList =( model.Where(t => t.createdDate > from && t.createdDate < to)).ToList();

                }*/
            //indicates the size of list
            int pageSize = 5;
            //set page to one is there is no value, ??  is called the null-coalescing operator.
            int pageNumber = (page ?? 1);
            //return the Model data with paged

            AIModel mod = new AIModel();
            Pagination viewModel = new Pagination();
            IPagedList<AIModel> pagedList = modelList.ToPagedList(pageNumber, pageSize);
            
            viewModel.AIModel = mod;
            viewModel.PagedListModels = pagedList;
            @ViewBag.totalModels = aIService.getAllModels().Count;
            

            return View(viewModel);
            }
            else
            {
                return LocalRedirect("/");
            }

           
        }
        

    

        [HttpPost]
        public async Task<IActionResult> CreateModel(Pagination mods)
        {
            Guid id = await aIService.createProject(mods.AIModel.projectName);

            return LocalRedirect("/AIModel/TestImages?projectId=" + id.ToString());
        }

        public IActionResult TestImages(Guid projectId)
        {
            @ViewBag.ID = projectId;
        
            var mods = aIService.getAllModels();
            for (int i = 0; i < mods.Count; i++)
            {
                if (mods[i].projectId.Equals(projectId))
                {
                    @ViewBag.Name = mods[i].projectName ;
                }
            }
          
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
