using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BeanBag.Services
{
    public interface IAIService
    {
        public string predict(string imageURL);

        public Task<Guid> createProject(string modelName);

        public void uploadTestImages(List<string> imageUrls, string[] tags, Guid projectId);
    }
}
