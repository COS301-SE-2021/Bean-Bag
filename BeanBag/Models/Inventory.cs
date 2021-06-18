using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace BeanBag.Models
{
    // This class is responsible for handling the inventory data for the application
    public class Inventory
    {
        // The Id field is a unique identifier for a specific inventory
        [Key]
        public Guid Id { get; set; }

        // The name of the Inventory. Required to have a value
        [Required]
        [DisplayName("Inventory Name")]
        public string name { get; set; }

        public string userId { get; set; }
    }
}
