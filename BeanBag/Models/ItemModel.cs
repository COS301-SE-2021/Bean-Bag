using System;
using System.Collections.Generic;
using System.Data.Entity;

namespace BeanBag.Models
{
    /*
   * This class is used to get and set all the variables related to the Item in the users inventory
   */
    public class ItemModel
    {
        public string InventoryId { get; set; }
        public string ItemName { get; set; }
        public string ItemType { get; set; }
        public DateTime ScanDate { get; set; }
        
        public virtual List<ItemModel> Items { get; set; }
    }
}
