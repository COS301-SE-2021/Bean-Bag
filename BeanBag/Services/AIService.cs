using BeanBag.Database;
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

        private readonly string trainingKey = "3fcb002210614500aaf87a89c79603d1";
        private readonly string projectId;
        private readonly string projectName;

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
            trainingClient = new CustomVisionTrainingClient(new Microsoft.Azure.CognitiveServices.Vision.CustomVision.Training.ApiKeyServiceClientCredentials(trainingKey))
            {
                Endpoint = endpoint
            };

            _db = db;
            

            //predictionClient = new CustomVisionPredictionClient
            //    (new Microsoft.Azure.CognitiveServices.Vision.CustomVision.Prediction.ApiKeyServiceClientCredentials(clothingModelPredictionKey))
            //{
            //    Endpoint = predictionUrl
            //};
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

        public string predict(string imageURL)
        {
            //var result = predictionClient.ClassifyImageUrl(new Guid(clothingModelProjectId), clothingModelPredictionName,
            //    new Microsoft.Azure.CognitiveServices.Vision.CustomVision.Prediction.Models.ImageUrl(imageURL));

            //return result.Predictions[0].TagName;
            return "";
        }
    }
}
