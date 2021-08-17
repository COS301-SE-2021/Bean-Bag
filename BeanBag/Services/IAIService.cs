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

        public Task<Guid> createProject(string projectName);

        public void deleteProject(Guid projectId);

        public void uploadTestImages(List<string> imageUrls, string[] tags, Guid projectId);

        public void trainModel(Guid projectId);

        public void deleteIteration(Guid iterationId);

        //public List<AIModelVersions> getAllIterations();

        public List<AIModelVersions> getAllAvailableIterations();

        public List<AIModel> getAllModels();

        public List<AIModelVersions> getProjectIterations(Guid projectId);

        public AIModelVersions getIteration(Guid iterationId);

        public void publishIteration(Guid projectId, Guid iterationId);

        public void unpublishIteration(Guid projectId, Guid iterationId);

    }
}
