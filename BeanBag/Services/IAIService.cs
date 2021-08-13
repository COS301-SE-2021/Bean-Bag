using BeanBag.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BeanBag.Services
{
    public interface IAIService
    {
        public string predict(Guid projectId, string iterationName, string imageURL);

        public Task<Guid> createProject(string modelName);

        public void uploadTestImages(List<string> imageUrls, string[] tags, Guid projectId);

        public void trainModel(Guid projectId);

        public List<AIModelVersions> getAllIterations();

        public List<AIModelVersions> getAllAvailableIterations();

        public List<AIModel> getAllModels();

        public Task<List<AIModelVersions>> getIterations(Guid projectId);

        public AIModelVersions getIteration(Guid iterationId);

        public void publishIteration(Guid projectId, Guid iterationId);

        public void unpublishIteration(Guid projectId, Guid iterationId);

    }
}
