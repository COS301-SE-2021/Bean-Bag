using System;

namespace BeanBag.Models
{
    // This class is used to get and set all the variables related to the Item in the users inventory.
    public class ItemModel
    { 
        /* This is the unique primary key ID used to identify every different inventory it shows the
         * ID of the inventory in which the current item exists. */
        public string InventoryId { get; set; }

        // This is the itemName of the current item.
        public string ItemName { get; set; }

        /* This pertains to the type of item, which is mostly used to relate it to the AI model used on it
         * to end users,for example and item of type furniture will use a Furniture AI model to analyze it.*/
        public string ItemType { get; set; }

        // This is the date the item was added to the inventory via and uploaded picture and AI analysis.
        public DateTime ScanDate { get; set; }
        
        // public string ItemID {get; set; }     to discuss with group whether to add??
    }
}
