using BeanBag.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BeanBag.Services
{
    // This class is an interface for the AI service.
    public interface IAiService
    {
        public string Predict(Guid projectId, string iterationName, string imageUrl);

        public Task<Guid> CreateProject(string projectName);

        public void DeleteProject(Guid projectId);

        public void UploadTestImages(List<string> imageUrls, string[] tags, Guid projectId);

        public void TrainModel(Guid projectId);

        public void DeleteIteration(Guid iterationId);
        
        public List<AIModelVersions> GetAllAvailableIterations();

        public List<AIModel> GetAllModels();

        public List<AIModelVersions> GetProjectIterations(Guid projectId);

        public AIModelVersions GetIteration(Guid iterationId);

        public void PublishIteration(Guid projectId, Guid iterationId);

        public void UnpublishIteration(Guid projectId, Guid iterationId);

        public List<AIModelVersions> GetAllAvailableIterationsOfModel(Guid projectId);

    }
}
