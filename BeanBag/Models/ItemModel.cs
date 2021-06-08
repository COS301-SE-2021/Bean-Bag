using System;
namespace BeanBag.Models
{
    /*
   * This class is used to get and set all the variables related to the Item in the users inventory
   */
    public class ItemModel
    {
        //public byte[] picture { get; set; }
        public string InventoryId { get; set; }
        public string ItemName { get; set; }
        public string ItemType { get; set; }
        public DateTime ScanDate { get; set; }

        //public virtual ICollection

    }
}
