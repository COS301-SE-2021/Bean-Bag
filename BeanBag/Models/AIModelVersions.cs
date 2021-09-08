using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BeanBag.Models
{
    public class AIModelVersions
    {
        [Key]
        public Guid iterationId { get; set; }
       
        [StringLength(20, ErrorMessage = "Tenant name length can't be more than 20.")]
        public string iterationName { get; set; }
        public bool availableToUser { get; set; }
        public string status { get; set; }
        [Required]
        public Guid projectId { get; set; }
        [ForeignKey("projectId")]
        public virtual AIModel AIModel { get; set; }
        public string predictionUrl { get; set; }
    }
}
