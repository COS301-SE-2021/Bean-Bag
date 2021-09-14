using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BeanBag.Models.Helper_Models
{
    public class ModelTag
    {
        public Guid Id { get; set; }
        public string name { get; set; }

        public int imageCount { get; set; }

        public Guid projectId { get; set; }
    }
}
