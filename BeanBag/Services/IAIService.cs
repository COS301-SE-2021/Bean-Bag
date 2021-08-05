using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BeanBag.Services
{
    public interface IAIService
    {
        public string predict(string imageURL);

        public string createProject(string modelName);
    }
}
