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
        private readonly string endpoint = "https://uksouth.api.cognitive.microsoft.com/";

        private readonly string key = "3fcb002210614500aaf87a89c79603d1";
        private readonly string predictionResourceId = "/subscriptions/5385f64c-2176-4307-bc1b-1cc4ee7f36e3/resourceGroups/Bean-Bag-Resource-Group/providers/Microsoft.CognitiveServices/accounts/Bean-Bag-Platform-AI-Models";

        private CustomVisionTrainingClient trainingClient;
        private CustomVisionPredictionClient predictionClient;

        private readonly DBContext _db;


        ////Furniture Model
        //private readonly string furnitureModelPredictionKey = "f05b67634cc3441492a07f32553d996a";
        //private readonly string furnitureModelProjectId = "377f08bf-2813-43cd-aa41-b0e623b2beec";
        //private readonly string furnitureModelPredictionName = "Iteration1";

        ////Clothing Model
        //private readonly string clothingModelPredictionKey = "3fcb002210614500aaf87a89c79603d1";
        //private readonly string clothingModelProjectId = "8c37a1ca-7ede-43ab-9a92-150d4e6c8fdc";
        //private readonly string clothingModelPredictionName = "ClothingMiniModelV1.0";

        //MAKE SURE TO REMOVE THIS TRAINING KEY. IT IS LIKE THE PASSWORD FOR OUR AI PROJECTS


        public AIService(DBContext db)
        {
            trainingClient = new CustomVisionTrainingClient(new Microsoft.Azure.CognitiveServices.Vision.CustomVision.Training.ApiKeyServiceClientCredentials(key))
            {
                Endpoint = endpoint
            };

            _db = db;
            

            predictionClient = new CustomVisionPredictionClient
                (new Microsoft.Azure.CognitiveServices.Vision.CustomVision.Prediction.ApiKeyServiceClientCredentials(key))
            {
                Endpoint = endpoint
            };
        }

        public string predict(Guid projectId, string iterationName, string imageURL)
        {
            var result = predictionClient.ClassifyImageUrl(projectId, iterationName,
                new Microsoft.Azure.CognitiveServices.Vision.CustomVision.Prediction.Models.ImageUrl(imageURL));

            //string itemType = "";

            //foreach(var prediction in result.Predictions)
            //{
            //    if (prediction.Probability > 0.9)
            //        itemType += prediction.TagName + ",";

            //}

            //if (itemType.Length > 0)
            //    itemType = itemType.Remove(itemType.Length - 1, 1);

            //return itemType;

            return result.Predictions[0].TagName;
        }


        public async Task<Guid> createProject(string modelName)
        {
            Project newProject = await trainingClient.CreateProjectAsync(modelName);

            AIModel newModel = new AIModel()
            {
                projectName = modelName,
                projectId = newProject.Id
            };

            await _db.AIModels.AddAsync(newModel);

            _db.SaveChanges();

            return newModel.projectId;
        }

        
        public async void uploadTestImages(List<string> imageUrls, string[] tags, Guid projectId)
        {
            if(trainingClient.GetProject(projectId) != null)
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
            else
            {
                // Throw exception that project does not exist
            }
            
        }

        public void trainModel(Guid projectId)
        {
            var iteration = trainingClient.TrainProject(projectId);
            string projectName = trainingClient.GetProject(projectId).Name;

            AIModelVersions newModelVersion = new AIModelVersions()
            {
                iterationName = projectName + " - Iteration 1",
                availableToUser = false,
                iterationId = iteration.Id,
                status = iteration.Status.ToString(),
                projectId = projectId
            };

            _db.AIModelIterations.Add(newModelVersion);
            _db.SaveChanges();
        }

        public async Task<List<AIModelVersions>> getIterations(Guid projectId)
        {
            var iterations = trainingClient.GetIterations(projectId);

            if (iterations.Count == 0)
                return new List<AIModelVersions>();

            foreach(var iteration in iterations)
            {
                AIModelVersions x = await _db.AIModelIterations.FindAsync(iteration.Id);
                x.status = iteration.Status;
                _db.Update(x);
            }
            await _db.SaveChangesAsync();

            return (from i in _db.AIModelIterations where i.projectId.Equals(projectId) select i).ToList();
        }

        public List<AIModelVersions> getAllIterations()
        {
            return _db.AIModelIterations.ToList();
        }

        public List<AIModelVersions> getAllAvailableIterations()
        {
            return (from i in _db.AIModelIterations where i.availableToUser.Equals(true) select i).ToList();
        }

        public List<AIModel> getAllModels()
        {
            return _db.AIModels.ToList();
        }

        public AIModelVersions getIteration(Guid iterationId)
        {
            return _db.AIModelIterations.Find(iterationId);
        }

        public void publishIteration(Guid projectId, Guid iterationId)
        {
            string iterationName = trainingClient.GetIteration(projectId, iterationId).Name;
            trainingClient.PublishIteration(projectId, iterationId, iterationName, predictionResourceId);

            var iteration = _db.AIModelIterations.Find(iterationId);
            iteration.availableToUser = true;
            _db.AIModelIterations.Update(iteration);
            _db.SaveChanges();
        }

        public async void unpublishIteration(Guid projectId, Guid iterationId)
        {
            await trainingClient.UnpublishIterationAsync(projectId, iterationId);

            _db.AIModelIterations.Find(iterationId).availableToUser = false;
            _db.SaveChanges();
        }
    }
}
