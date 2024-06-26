﻿using BeanBag.Models;
using BeanBag.Models.Helper_Models;
using Microsoft.Azure.CognitiveServices.Vision.CustomVision.Training.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BeanBag.Services
{
    public interface IAIService
    {
        public List<AIPrediction> predict(Guid projectId, string iterationName, string imageURL);

        public Task<Guid> createProject(string projectName, string description, string tenantId);

        public void deleteProject(Guid projectId);

        public void editProject(Guid projectId, string projectName, string description);

        public void uploadImages(List<string> imageUrls, string[] tags, Guid projectId);

        public void trainModel(Guid projectId);

        public void deleteIteration(Guid iterationId);

        //public List<AIModelVersions> getAllIterations();

        public List<AIModelVersions> getAllAvailableIterations();

        public List<AIModel> getAllModels(string tenantId);

        public List<AIModelVersions> getProjectIterations(Guid projectId);

        public AIModelVersions getIteration(Guid iterationId);

        public void publishIteration(Guid projectId, Guid iterationId);

        public void unpublishIteration(Guid projectId, Guid iterationId);

        public List<AIModelVersions> getAllAvailableIterationsOfModel(Guid projectId);

        public IterationPerformance getModelVersionPerformance(Guid projectId, Guid iterationId);

        public List<AIModelVersionTagPerformance> getPerformancePerTags(Guid projectId, Guid iterationId);

        public IList<Tag> getIterationTags(Guid projectId, Guid iterationId);

        public AIModel getModel(Guid projectId);

        public void EditIteration(Guid iterationId, string description);

        public int? getImageCount(Guid projectId);

        public List<string> AIModelRecommendations(Guid projectId);

        public IList<Tag> getModelTags(Guid projectId);

        public void deleteModelTag(Guid tagId, Guid projectId, int imageCount);

        public void updateImageCount(Guid projectId);

        public List<AIModelVersions> getAllIterations();
    }
}
