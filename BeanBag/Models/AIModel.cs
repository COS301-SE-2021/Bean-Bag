using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BeanBag.Models
{
    public class AIModel
    {
        [Key]
        public Guid projectId { get; set; }

        public string projectName { get; set; }

        //public string description { get; set; }

        //public Guid tenantId { get; set; }

        //[ForeignKey("tenantId")]
        //public virtual Tenant tenant { get; set; }
    }
}
