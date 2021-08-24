using BeanBag.Database;
using BeanBag.Models;
using Microsoft.Azure.CognitiveServices.Vision.CustomVision.Prediction;
using Microsoft.Azure.CognitiveServices.Vision.CustomVision.Training;
using Microsoft.Azure.CognitiveServices.Vision.CustomVision.Training.Models;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BeanBag.Services
{
    // The AI service class is used by the AIModel controller to create and train AI models as well as publish iterations of the model
    public class AiService : IAiService
    {
        // Variables
        private readonly CustomVisionTrainingClient _trainingClient;
        private readonly CustomVisionPredictionClient _predictionClient;
        private readonly DBContext _db;
        private readonly IBlobStorageService _blob;
        private readonly string _resourceId;

        //Constructor
        public AiService(DBContext db, IBlobStorageService blob, IConfiguration config)
        {
            string key = config.GetValue<string>("CustomVision:Key");
            _resourceId = config.GetValue<string>("CustomVision:ResourceId");

            _trainingClient = new CustomVisionTrainingClient(new Microsoft.Azure.CognitiveServices.Vision.CustomVision.Training.ApiKeyServiceClientCredentials(key))
            {
                Endpoint = config.GetValue<string>("CustomVision:Endpoint")
            };

            _db = db;
            _blob = blob;

            _predictionClient = new CustomVisionPredictionClient
                (new Microsoft.Azure.CognitiveServices.Vision.CustomVision.Prediction.ApiKeyServiceClientCredentials(key))
            {
                Endpoint = config.GetValue<string>("CustomVision:Endpoint")
            };
        }

        // This method is used to return the tags (item type) from a specified model for an item image 
        public string Predict(Guid projectId, string iterationName, string imageUrl)
        {
            if (projectId == Guid.Empty)
                throw new Exception("Project Id is null");

            if (_trainingClient.GetProject(projectId) == null)
                throw new Exception("No Custom Vision project exists with Guid " + projectId.ToString());

            if (iterationName.Equals("") || iterationName.Equals(" "))
                throw new Exception("Invalid iteration name");

            if (imageUrl.Equals("") || imageUrl.Equals(" "))
                throw new Exception("Invalid image url");

            //This is used to check that the image url comes from a valid source that being the polaris blob storage
            if (!imageUrl.Contains("https://polarisblobstorage.blob.core.windows.net/itemimages/"))
                throw new Exception("Image url comes from invalid source");

            try 
            {             
                var result = _predictionClient.ClassifyImageUrl(projectId, iterationName,
                new Microsoft.Azure.CognitiveServices.Vision.CustomVision.Prediction.Models.ImageUrl(imageUrl));

                string itemType = "";

                foreach (var prediction in result.Predictions)
                {
                    // The probability needs to be above 90% to be deemed accurate by the model
                    //if (prediction.Probability > 0.9)
                    itemType += prediction.TagName + ",";

                }

                if (itemType.Length > 0)
                    itemType = itemType.Remove(itemType.Length - 1, 1);

                return itemType;
            }
            catch(Exception e)
            {
                throw new Exception(e.ToString());
            }
        }

        // This method is used to create a new project (model) in custom vision
        public async Task<Guid> CreateProject(string projectName)
        {
            if (projectName.Equals("") || projectName.Equals(" "))
                throw new Exception("Invalid project name");

            try
            {
                Project newProject = await _trainingClient.CreateProjectAsync(projectName);

                AIModel newModel = new AIModel()
                {
                    projectName = projectName,
                    projectId = newProject.Id
                };

                await _db.AIModels.AddAsync(newModel);

                await _db.SaveChangesAsync();

                return newModel.projectId;
            }
            catch(Exception e)
            {
                throw new Exception(e.ToString());
            }
            
        }

        // This method deletes a custom vision project from the DB and custom vision service
        public void DeleteProject(Guid projectId)
        {
            if (projectId == Guid.Empty)
                throw new Exception("Project id is null");

            try
            {
                // 1) removing test images from blob storage
                _blob.DeleteTestImageFolder(projectId.ToString());

                // 2) Deleting from Custom Vision and iterations
                var iterations = _trainingClient.GetIterations(projectId);
                foreach (var i in iterations)
                    DeleteIteration(i.Id);

                _trainingClient.DeleteProject(projectId);

                // 3) Deleting from DB
                AIModel model = _db.AIModels.Find(projectId);
                _db.AIModels.Remove(model);
                _db.SaveChanges();

            }
            catch(Exception e)
            {
                throw new Exception(e.ToString());
            }
        }

        // This method is used to upload a set of test images into the Azure blob storage and then into the custom vision project
        public async void UploadTestImages(List<string> imageUrls, string[] tags, Guid projectId)
        {
            if (projectId == Guid.Empty)
                throw new Exception("Project id is null");

            if (await _trainingClient.GetProjectAsync(projectId) == null)
                throw new Exception("Custom vision project does not exist with project id " + projectId.ToString());

            try
            {
                List<Guid> imageTagsId = new List<Guid>();
                foreach(var tag in tags)
                {
                    imageTagsId.Add((await _trainingClient.CreateTagAsync(projectId, tag)).Id);
                }

                List<ImageUrlCreateEntry> images = new List<ImageUrlCreateEntry>();

                foreach(var url in imageUrls)
                {
                    images.Add(new ImageUrlCreateEntry(url, imageTagsId));
                }

                await _trainingClient.CreateImagesFromUrlsAsync(projectId, new ImageUrlCreateBatch(images));
            }
            catch(Exception e)
            {
                throw new Exception(e.ToString());
            }
            
        }

        // This method trains the model with the test images and creates a new iteration
        public void TrainModel(Guid projectId)
        {
            if (projectId == Guid.Empty)
                throw new Exception("Project id is null");

            if (_trainingClient.GetProject(projectId) == null)
                throw new Exception("Custom Vision project does not exist with projectId " + projectId.ToString());

            try
            {
                var iteration = _trainingClient.TrainProject(projectId);
                // string projectName = _trainingClient.GetProject(projectId).Name;

                AIModelVersions newModelVersion = new AIModelVersions()
                {
                    iterationName = iteration.Name,
                    availableToUser = false,
                    iterationId = iteration.Id,
                    status = iteration.Status,
                    projectId = projectId
                };

                _db.AIModelIterations.Add(newModelVersion);
                _db.SaveChanges();
            }
            catch(Exception e)
            {
                throw new Exception(e.ToString());
            }
            
        }

        // This method deletes an iteration from the custom vision service as well as from the DB
        public void DeleteIteration(Guid iterationId)
        {
            if (iterationId == Guid.Empty)
                throw new Exception("Iteration id is null");

            AIModelVersions iteration = _db.AIModelIterations.Find(iterationId);

            if (iteration == null)
                throw new Exception("Iteration not found with iteration id " + iterationId.ToString());

            try
            {
                if(iteration.availableToUser)
                    _trainingClient.UnpublishIteration(iteration.projectId, iterationId);

                _trainingClient.DeleteIteration(iteration.projectId, iterationId);

                _db.AIModelIterations.Remove(iteration);
                _db.SaveChanges();
            }
            catch(Exception e)
            {
                throw new Exception(e.ToString());
            }
        }

        // This method returns all of the iterations related to a custom vision project
        public List<AIModelVersions> GetProjectIterations(Guid projectId)
        {
            if (projectId == Guid.Empty)
                throw new Exception("Project id is null");
            if (_trainingClient.GetProject(projectId) == null)
                throw new Exception("Custom Vision project does not exist with project id " + projectId.ToString());

            try
            {
                var iterations = _trainingClient.GetIterations(projectId);

                if (iterations.Count == 0)
                    return new List<AIModelVersions>();

                UpdateProjectIterationsStatus(iterations.ToList());

                return (from i in _db.AIModelIterations where i.projectId.Equals(projectId) select i).ToList();
            }
            catch(Exception e)
            {
                throw new Exception(e.ToString());
            }
            
        }

        // This method is used to update the project iterations status in the DB
        private void UpdateProjectIterationsStatus(List<Iteration> iterations)
        {
            if (iterations.Count == 0)
                throw new Exception("List of iterations is empty");

            foreach (var iteration in iterations)
            {
                AIModelVersions x = _db.AIModelIterations.Find(iteration.Id);
                x.status = iteration.Status;
                _db.Update(x);
            }
            _db.SaveChanges();
        }

        // This method is used to retrieve all available to user iterations from the DB
        public List<AIModelVersions> GetAllAvailableIterations()
        {
            try
            {
                return (from i in _db.AIModelIterations where i.availableToUser.Equals(true) select i).ToList();
            }
            catch(Exception e)
            {
                throw new Exception(e.ToString());
            }
            
        }

        // This method is used to retrieve all of the AI Model projects in the DB
        public List<AIModel> GetAllModels()
        {
            try 
            {
                return _db.AIModels.ToList();
            }
            catch(Exception e)
            {
                throw new Exception(e.ToString());
            }
            
        }

        // This method is used to retrieve a single iteration from the DB
        public AIModelVersions GetIteration(Guid iterationId)
        {
            if (iterationId == Guid.Empty)
                throw new Exception("Iteration Id is null.");
            
            try 
            {
                return _db.AIModelIterations.Find(iterationId);
            }
            catch(Exception e)
            {
                throw new Exception(e.ToString());
            }
            
        }

        // This method is used to publish an iteration thus making it available to the user 
        public void PublishIteration(Guid projectId, Guid iterationId)
        {
            if (projectId == Guid.Empty)
                throw new Exception("Project id is null");
            if (iterationId == Guid.Empty)
                throw new Exception("Iteration id is null");

            try
            {
                string iterationName = _trainingClient.GetIteration(projectId, iterationId).Name;
                _trainingClient.PublishIteration(projectId, iterationId, iterationName, _resourceId);

                var iteration = _db.AIModelIterations.Find(iterationId);
                iteration.availableToUser = true;
                _db.AIModelIterations.Update(iteration);
                _db.SaveChanges();
            }
            catch(Exception e)
            {
                throw new Exception(e.ToString());
            }
            
        }

        // This method is used to unpublish an iteration thus making it unavailable to the user
        public void UnpublishIteration(Guid projectId, Guid iterationId)
        {
            if (projectId == Guid.Empty)
                throw new Exception("Project id is null");
            if (iterationId == Guid.Empty)
                throw new Exception("Iteration id is null");

            try
            {
                _trainingClient.UnpublishIteration(projectId, iterationId);

                _db.AIModelIterations.Find(iterationId).availableToUser = false;
                _db.SaveChanges();
            }          
            catch(Exception e)
            {
                throw new Exception(e.ToString());
            }
        }

        // This method returns all iterations available to the user that belongs to a model
        public List<AIModelVersions> GetAllAvailableIterationsOfModel(Guid projectId)
        {
            return _db.AIModelIterations.Where(i => i.availableToUser.Equals(true) && i.projectId.Equals(projectId)).ToList();
        }
    }
}
