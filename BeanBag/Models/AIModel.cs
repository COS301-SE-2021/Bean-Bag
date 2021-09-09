using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BeanBag.Models
{
    public class AIModel
    {
        [Key]
        public Guid Id { get; set; }

        public string name { get; set; }

        public DateTime dateCreated { get; set; }
    }
}
