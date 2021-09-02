using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace BeanBag.Models
{
    public class AIModel
    {
        [Key]
        public Guid projectId { get; set; }

        [Required]
        [DisplayName("Model Name")]
        public string projectName { get; set; }
    }
}
