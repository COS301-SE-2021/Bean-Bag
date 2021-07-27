using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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

        public DateTime entryDate { get; set; }

        [Required]
        [DisplayName("Item Price")]
        [Range(0, int.MaxValue, ErrorMessage ="Price needs to be positive")]
        public double price { get; set; }

        [Required]
        [DisplayName("Item Quantity")]
        [Range(0, int.MaxValue, ErrorMessage = "Quantity needs to be positive")]
        public int quantity { get; set; }

        [DisplayName("Sold Status")]
        public bool isSold { get; set; }

        [DisplayName("Sold Date")]
        public DateTime soldDate { get; set; }
    }
}
