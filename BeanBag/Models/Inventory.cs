using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace BeanBag.Models
{
    // This class is responsible for handling the inventory data for the application
    public partial class Inventory
    {
        // The Id field is a unique identifier for a specific inventory
        [Key]
        public Guid Id { get; set; }

        // The name of the Inventory. Required to have a value
        [Required]
        [DisplayName("Inventory Name")]
        [StringLength(100, ErrorMessage = "Inventory name length can't be more than 100.")]
        public string name { get; set; }
        public string userId { get; set; }
        
        [StringLength(250, ErrorMessage = "Description name length can't be more than 250.")]
        public string description { get; set; }
        public DateTime createdDate { get; set; }
        
        public bool publicToTenant { get; set; }
    }
    
}
