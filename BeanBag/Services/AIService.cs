﻿using BeanBag.Database;
using BeanBag.Models;
using BeanBag.Models.Helper_Models;
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
    public class AIService : IAIService
    {
        // Variables
        private CustomVisionTrainingClient trainingClient;
        private CustomVisionPredictionClient predictionClient;
        private readonly DBContext _db;
        private readonly IBlobStorageService _blob;
        private string resourceId;

        //Constructor
        public AIService(DBContext db, IBlobStorageService blob, IConfiguration config)
        {
            string key = config.GetValue<string>("CustomVision:Key");
            resourceId = config.GetValue<string>("CustomVision:ResourceId");

            trainingClient = new CustomVisionTrainingClient(new Microsoft.Azure.CognitiveServices.Vision.CustomVision.Training.ApiKeyServiceClientCredentials(key))
            {
                Endpoint = config.GetValue<string>("CustomVision:Endpoint")
            };

            _db = db;
            _blob = blob;

            predictionClient = new CustomVisionPredictionClient
                (new Microsoft.Azure.CognitiveServices.Vision.CustomVision.Prediction.ApiKeyServiceClientCredentials(key))
            {
                Endpoint = config.GetValue<string>("CustomVision:Endpoint")
            };
        }

        // This method is used to return the tags (item type) from a specified model for an item image 
        public List<AIPrediction> predict(Guid projectId, string iterationName, string imageURL)
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

            try
            {
                var result = predictionClient.ClassifyImageUrl(projectId, iterationName,
                new Microsoft.Azure.CognitiveServices.Vision.CustomVision.Prediction.Models.ImageUrl(imageURL));

                List<AIPrediction> listPred = new List<AIPrediction>();

                foreach (var prediction in result.Predictions)
                {
                    if (prediction.Probability > 0.75)
                    {
                        listPred.Add(new AIPrediction
                        {
                            tagName = prediction.TagName,
                            percentage = prediction.Probability * 100
                        });
                    }


                }

                return listPred;
            }
            catch (Exception e)
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
                    name = projectName,
                    Id = newProject.Id
                };

                await _db.AIModels.AddAsync(newModel);

                _db.SaveChanges();

                return newModel.Id;
            }
            catch (Exception e)
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
            catch (Exception e)
            {
                throw new Exception(e.ToString());
            }
        }

        // This method edits the name of the custom vision project as well as the description of the model. Reflects in the DB
        public void editProject(Guid projectId, string projectName, string description)
        {
            if (projectId == Guid.Empty)
                throw new Exception("Project id is null");

            //Changing the name in custom vision
            trainingClient.GetProject(projectId).Name = projectName;

            var project = _db.AIModels.Find(projectId);

            project.name = projectName;
            //project.description = description;
            _db.AIModels.Update(project);

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
                foreach (var tag in tags)
                {
                    imageTagsId.Add(trainingClient.CreateTag(projectId, tag).Id);
                }

                List<ImageUrlCreateEntry> images = new List<ImageUrlCreateEntry>();

                foreach (var url in imageUrls)
                {
                    images.Add(new ImageUrlCreateEntry(url, imageTagsId, null));
                }

                await trainingClient.CreateImagesFromUrlsAsync(projectId, new ImageUrlCreateBatch(images));
            }
            catch (Exception e)
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
                //var iteration = trainingClient.TrainProject(projectId, "Advanced");
                var iteration = trainingClient.TrainProject(projectId);
                string projectName = trainingClient.GetProject(projectId).Name;

                AIModelVersions newModelVersion = new AIModelVersions()
                {
                    Name = "Version " + iteration.Name.Substring(iteration.Name.Length-1),
                    availableToUser = false,
                    Id = iteration.Id,
                    status = iteration.Status.ToString(),
                    projectId = projectId
                };

                _db.AIModelIterations.Add(newModelVersion);
                _db.SaveChanges();
            }
            catch (Exception e)
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
                if (iteration.availableToUser)
                    trainingClient.UnpublishIteration(iteration.projectId, iterationId);

                trainingClient.DeleteIteration(iteration.projectId, iterationId);

                _db.AIModelIterations.Remove(iteration);
                _db.SaveChanges();
            }
            catch (Exception e)
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
            catch (Exception e)
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

        // This method is used to retrieve all available to user iterations from the DB
        public List<AIModelVersions> getAllAvailableIterations()
        {
            try
            {
                return (from i in _db.AIModelIterations where i.availableToUser.Equals(true) select i).ToList();
            }
            catch (Exception e)
            {
                throw new Exception(e.ToString());
            }

        }

        // This method is used to retrieve all of the AI Model projects in the DB
        public List<AIModel> getAllModels()
        {
            try
            {
                return _db.AIModels.ToList();
            }
            catch (Exception e)
            {
                throw new Exception(e.ToString());
            }

        }

        // This method is used to retrieve a single iteration from the DB
        public AIModelVersions getIteration(Guid iterationId)
        {
            if (iterationId == Guid.Empty)
                throw new Exception("Iteration Id is null.");

            try
            {
                return _db.AIModelIterations.Find(iterationId);
            }
            catch (Exception e)
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
                trainingClient.PublishIteration(projectId, iterationId, iterationName, resourceId);

                var iteration = _db.AIModelIterations.Find(iterationId);
                iteration.availableToUser = true;
                _db.AIModelIterations.Update(iteration);
                _db.SaveChanges();
            }
            catch (Exception e)
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
            catch (Exception e)
            {
                throw new Exception(e.ToString());
            }
        }

        // This method returns all iterations available to the user that belongs to a model
        public List<AIModelVersions> getAllAvailableIterationsOfModel(Guid projectId)
        {
            return _db.AIModelIterations.Where(i => i.availableToUser.Equals(true) && i.projectId.Equals(projectId)).ToList();
        }

        // This method returns an AI Model iteration performace (accuracy, precision, recall, AP)
        public IterationPerformance getModelVersionPerformance(Guid projectId, Guid iterationId)
        {
            if (projectId == Guid.Empty)
                throw new Exception("Project id is null");

            if (iterationId == Guid.Empty)
                throw new Exception("Iteration id is null");

            return trainingClient.GetIterationPerformance(projectId, iterationId);
        }

        // This method returns an AI Model iteration image tag performace (accuracy, precision, recall, ap, imageCount)
        public List<AIModelVersionTagPerformance> getPerformancePerTags(Guid projectId, Guid iterationId, IterationPerformance iterationPerformance)
        {
            if (projectId == Guid.Empty)
                throw new Exception("Project id is null");

            if (iterationId == Guid.Empty)
                throw new Exception("Iteration id is null");

            List<AIModelVersionTagPerformance> returnList = new List<AIModelVersionTagPerformance>();

            foreach (var tag in iterationPerformance.PerTagPerformance)
            {
                AIModelVersionTagPerformance addToList = new AIModelVersionTagPerformance()
                {
                    tagId = tag.Id,
                    precision = tag.Precision,
                    recall = tag.Recall,
                    averagePrecision = tag.AveragePrecision,
                    tagName = tag.Name
                };

                returnList.Add(addToList);
            }


            IList<Tag> imageTags = trainingClient.GetTags(projectId, iterationId);
            foreach (var tag in imageTags)
            {
                for (int i = 0; i < returnList.Count; i++)
                {
                    for (int j = 0; j < imageTags.Count; j++)
                    {
                        if (returnList[i].tagId == imageTags[j].Id)
                        {
                            returnList[i].imageCount = imageTags[j].ImageCount;
                            break;
                        }
                    }
                }
            }

            return returnList;
        }

        public IList<Tag> getIterationTags(Guid projectId, Guid iterationId)
        {
            return trainingClient.GetTags(projectId, iterationId);
        }

        public List<string> AIModelRecommendations(Guid projectId)
        {
            List<string> recommendations = new List<string>();

            IList<Tag> tags = trainingClient.GetTags(projectId);

            //Checking if their are more than one tag in a model
            if(tags.Count == 0 || recommendations.Count == 1)
            {
                recommendations.Add(trainingClient.GetProject(projectId).Name + " needs to have more than one tag.");
                return recommendations;
            }

            string minTag = tags[0].Name;
            string maxTag = tags[0].Name;
            int min = tags[0].ImageCount;
            int max = tags[0].ImageCount;

            foreach (var tag in tags)
            {
                //50 images at least for each tag made
                if (tag.ImageCount < 50)
                {
                    recommendations.Add(tag.Name + " needs to have more than 50 images associated with it.");
                }

                if (tag.ImageCount > max)
                    max = tag.ImageCount;
                if (tag.ImageCount < min)
                    min = tag.ImageCount;
            }

            //Ensuring balance data. That the max amount of images associated with a tag is not over double than the min amount of images associated with a tag
            if ((max / min) > 2)
                recommendations.Add(maxTag + " has more than double the amount of images than " + minTag + ". Add more images to " + minTag + " to make the model have more balanced data.");

            return recommendations;
        }

        public AIModel getModel(Guid projectId)
        {
            AIModel model = _db.AIModels.Find(projectId);
            return model;
        }
    }
}
