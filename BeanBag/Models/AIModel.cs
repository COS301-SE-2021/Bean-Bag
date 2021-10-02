using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BeanBag.Models
{
    public class AIModel
    {
        [Key]
        public Guid Id { get; set; }

        [StringLength(100, ErrorMessage = "AI Model name length can't be more than 100.")]
        public string name { get; set; }

        public DateTime dateCreated { get; set; }

        public string description { get; set; }

        public int? imageCount { get; set; }

        public string tenantId { get; set; }
    }
}
