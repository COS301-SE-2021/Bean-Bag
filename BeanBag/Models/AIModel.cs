using System;
using System.ComponentModel.DataAnnotations;

namespace BeanBag.Models
{
    public class AIModel
    {
        [Key]
        public Guid projectId { get; set; }

        [Required]
        public string projectName { get; set; }
    }
}
