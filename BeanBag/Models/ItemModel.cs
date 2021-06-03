using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BeanBag.Models
{
    /*
   * This class is used to get and set all the variables related to the Item in the users inventory
   */
    public class ItemModel
    {
        //public byte[] picture { get; set; }
        public string itemID { get; set; }
        public string inventoryID { get; set; }
        public string itemName { get; set; }
        public string itemType { get; set; }
        public DateTime scanDate { get; set; }

        //public virtual ICollection

    }
}
