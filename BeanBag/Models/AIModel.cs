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
        [StringLength(100, ErrorMessage = "Model name length can't be more than 100.")]
        public string projectName { get; set; }
    }
}
