using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

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
        
        [Required]
        [DisplayName("Inventory")]
        public Guid inventoryId { get; set; }

        [ForeignKey("inventoryId")]
        public virtual Inventory Inventory { get; set; }

        public string imageURL { get; set; }

        public string QRContents { get; set; }
    }
}
