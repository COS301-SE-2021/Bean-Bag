using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace BeanBag.Models
{
    public class AIModelVersions
    {
        [Key]
        public Guid iterationId { get; set; }

        public string iterationName { get; set; }

        public bool availableToUser { get; set; }

        public string status { get; set; }
        
        [Required]
        public Guid projectId { get; set; }

        public string predictionUrl { get; set; }
    }
}
