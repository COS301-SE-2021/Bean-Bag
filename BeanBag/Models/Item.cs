using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
namespace BeanBag.Models
{
    // This class is responsible for handling the items data for the application
    public class Item
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        [DisplayName("Item Name")]
        public string name { get; set; }

        [Required]
        [DisplayName("Item Type")]
        public string type { get; set; }

        public Guid inventoryId { get; set; }
    }
}
