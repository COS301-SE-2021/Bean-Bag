using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BeanBag.Models
{
    public class AIModel
    {
        [Key]
        public Guid projectId { get; set; }

        public string projectName { get; set; }
    }
}
