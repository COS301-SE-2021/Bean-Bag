﻿using BeanBag.Database;
using BeanBag.Models;
using Microsoft.Azure.CognitiveServices.Vision.CustomVision.Prediction;
using Microsoft.Azure.CognitiveServices.Vision.CustomVision.Training;
using Microsoft.Azure.CognitiveServices.Vision.CustomVision.Training.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BeanBag.Services
{
    public class AIService : IAIService
    {
        //MAKE SURE TO REMOVE THIS TRAINING KEY. IT IS LIKE THE PASSWORD FOR OUR AI PROJECTS
        private readonly string endpoint = "https://uksouth.api.cognitive.microsoft.com/";

        private readonly string key = "3fcb002210614500aaf87a89c79603d1";
        private readonly string predictionResourceId = "/subscriptions/5385f64c-2176-4307-bc1b-1cc4ee7f36e3/resourceGroups/Bean-Bag-Resource-Group/providers/Microsoft.CognitiveServices/accounts/Bean-Bag-Platform-AI-Models";

        private CustomVisionTrainingClient trainingClient;
        private CustomVisionPredictionClient predictionClient;

        private readonly DBContext _db;
        private readonly IBlobStorageService _blob;

        public AIService(DBContext db, IBlobStorageService blob)
        {
            trainingClient = new CustomVisionTrainingClient(new Microsoft.Azure.CognitiveServices.Vision.CustomVision.Training.ApiKeyServiceClientCredentials(key))
            {
                Endpoint = endpoint
            };

            _db = db;
            _blob = blob;

            predictionClient = new CustomVisionPredictionClient
                (new Microsoft.Azure.CognitiveServices.Vision.CustomVision.Prediction.ApiKeyServiceClientCredentials(key))
            {
                Endpoint = endpoint
            };
        }

        // This method is used to return the tags (item type) of an item image
        public string predict(Guid projectId, string iterationName, string imageURL)
        {
            if (projectId == Guid.Empty)
                throw new Exception("Project Id is null");

            if (trainingClient.GetProject(projectId) == null)
                throw new Exception("No Custom Vision project exists with Guid " + projectId.ToString());

            if (iterationName.Equals("") || iterationName.Equals(" "))
                throw new Exception("Invalid itertaion name");

            if (imageURL.Equals("") || imageURL.Equals(" "))
                throw new Exception("Invalid image url");

            //This is used to check that the image url comes from a valid source that being the polaris blob storage
            if (!imageURL.Contains("https://polarisblobstorage.blob.core.windows.net/itemimages/"))
                throw new Exception("Image url comes from invalid source");

            try {
                

                var result = predictionClient.ClassifyImageUrl(projectId, iterationName,
                new Microsoft.Azure.CognitiveServices.Vision.CustomVision.Prediction.Models.ImageUrl(imageURL));

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
        public async Task<Guid> createProject(string projectName)
        {
            if (projectName.Equals("") || projectName.Equals(" "))
                throw new Exception("Invalid project name");

            try
            {
                Project newProject = await trainingClient.CreateProjectAsync(projectName);

                AIModel newModel = new AIModel()
                {
                    projectName = projectName,
                    projectId = newProject.Id
                };

                await _db.AIModels.AddAsync(newModel);

                _db.SaveChanges();

                return newModel.projectId;
            }
            catch(Exception e)
            {
                throw new Exception(e.ToString());
            }
            
        }

        // This method deletes a custom vision project from the DB and custom vision service
        public void deleteProject(Guid projectId)
        {
            if (projectId == Guid.Empty)
                throw new Exception("Project id is null");

            try
            {
                // 1) removing test images from blob storage
                _blob.deleteTestImageFolder(projectId.ToString());

                // 2) Deleting from Custom Vision and iterations
                var iterations = trainingClient.GetIterations(projectId);
                foreach (var i in iterations)
                    deleteIteration(i.Id);

                trainingClient.DeleteProject(projectId);

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
        public async void uploadTestImages(List<string> imageUrls, string[] tags, Guid projectId)
        {
            if (projectId == Guid.Empty)
                throw new Exception("Project id is null");

            if (trainingClient.GetProject(projectId) == null)
                throw new Exception("Custom vision project does not exist with project id " + projectId.ToString());

            try
            {
                List<Guid> imageTagsId = new List<Guid>();
                foreach(var tag in tags)
                {
                    imageTagsId.Add(trainingClient.CreateTag(projectId, tag).Id);
                }

                List<ImageUrlCreateEntry> images = new List<ImageUrlCreateEntry>();

                foreach(var url in imageUrls)
                {
                    images.Add(new ImageUrlCreateEntry(url, imageTagsId, null));
                }

                await trainingClient.CreateImagesFromUrlsAsync(projectId, new ImageUrlCreateBatch(images));
            }
            catch(Exception e)
            {
                throw new Exception(e.ToString());
            }
            
        }

        // This method trains the model with the test images and creates a new iteration
        public void trainModel(Guid projectId)
        {
            if (projectId == Guid.Empty)
                throw new Exception("Project id is null");

            if (trainingClient.GetProject(projectId) == null)
                throw new Exception("Custom Vision project does not exist with projectId " + projectId.ToString());

            try
            {
                var iteration = trainingClient.TrainProject(projectId);
                string projectName = trainingClient.GetProject(projectId).Name;

                AIModelVersions newModelVersion = new AIModelVersions()
                {
                    iterationName = iteration.Name,
                    availableToUser = false,
                    iterationId = iteration.Id,
                    status = iteration.Status.ToString(),
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
        public void deleteIteration(Guid iterationId)
        {
            if (iterationId == Guid.Empty)
                throw new Exception("Iteration id is null");

            AIModelVersions iteration = _db.AIModelIterations.Find(iterationId);

            if (iteration == null)
                throw new Exception("Iteration not found with iteration id " + iterationId.ToString());

            try
            {
                if(iteration.availableToUser)
                    trainingClient.UnpublishIteration(iteration.projectId, iterationId);

                trainingClient.DeleteIteration(iteration.projectId, iterationId);

                _db.AIModelIterations.Remove(iteration);
                _db.SaveChanges();
            }
            catch(Exception e)
            {
                throw new Exception(e.ToString());
            }
        }

        // This method returns all of the iterations related to a custom vision project
        public List<AIModelVersions> getProjectIterations(Guid projectId)
        {
            if (projectId == Guid.Empty)
                throw new Exception("Project id is null");
            if (trainingClient.GetProject(projectId) == null)
                throw new Exception("Custom Vision project does not exist with project id " + projectId.ToString());

            try
            {
                var iterations = trainingClient.GetIterations(projectId);

                if (iterations.Count == 0)
                    return new List<AIModelVersions>();

                updateProjectIterationsStatus(iterations.ToList());

                return (from i in _db.AIModelIterations where i.projectId.Equals(projectId) select i).ToList();
            }
            catch(Exception e)
            {
                throw new Exception(e.ToString());
            }
            
        }

        // This method is used to update the project iterations status in the DB
        public void updateProjectIterationsStatus(List<Iteration> iterations)
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

        //public List<AIModelVersions> getAllIterations()
        //{
        //    return _db.AIModelIterations.ToList();
        //}

        // This method is used to retrieve all available to user iterations from the DB
        public List<AIModelVersions> getAllAvailableIterations()
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
        public List<AIModel> getAllModels()
        {
            try {
                return _db.AIModels.ToList();
            }
            catch(Exception e)
            {
                throw new Exception(e.ToString());
            }
            
        }

        // This method is used to retrieve a single iteration from the DB
        public AIModelVersions getIteration(Guid iterationId)
        {
            if (iterationId == Guid.Empty)
                throw new Exception(endpoint.ToString());
            
            try {
                return _db.AIModelIterations.Find(iterationId);
            }
            catch(Exception e)
            {
                throw new Exception(e.ToString());
            }
            
        }

        // This method is used to publish an iteration thus making it available to the user 
        public void publishIteration(Guid projectId, Guid iterationId)
        {
            if (projectId == Guid.Empty)
                throw new Exception("Project id is null");
            if (iterationId == Guid.Empty)
                throw new Exception("Iteration id is null");

            try
            {
                string iterationName = trainingClient.GetIteration(projectId, iterationId).Name;
                trainingClient.PublishIteration(projectId, iterationId, iterationName, predictionResourceId);

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
        public void unpublishIteration(Guid projectId, Guid iterationId)
        {
            if (projectId == Guid.Empty)
                throw new Exception("Project id is null");
            if (iterationId == Guid.Empty)
                throw new Exception("Iteration id is null");

            try
            {
                trainingClient.UnpublishIteration(projectId, iterationId);

                _db.AIModelIterations.Find(iterationId).availableToUser = false;
                _db.SaveChanges();
            }          
            catch(Exception e)
            {
                throw new Exception(e.ToString());
            }
        }

        public List<AIModelVersions> getAllAvailableIterationsOfModel(Guid projectId)
        {
            return _db.AIModelIterations.Where(i => i.availableToUser.Equals(true) && i.projectId.Equals(projectId)).ToList();
        }
    }
}
