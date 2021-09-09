using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BeanBag.Models.Helper_Models
{
    public class AIModelVersionTagPerformance
    {
        public Guid tagId { get; set; }
        public double precision { get; set; }
        public double recall { get; set; }
        public double? averagePrecision { get; set; }
        public int imageCount { get; set; }

        public string tagName { get; set; }
    }
}
