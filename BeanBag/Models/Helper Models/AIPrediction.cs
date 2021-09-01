using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BeanBag.Models
{
    // This class is used to associate a tag with a percentage to be displayed on the create item page
    public class AIPrediction
    {
        public string tagName { get; set; }

        public double percentage { get; set; }
    }
}
