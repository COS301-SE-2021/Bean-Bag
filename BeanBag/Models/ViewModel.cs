using System.Collections.Generic;

namespace BeanBag.Models
{
    public class ViewModel
    {
        
            public Inventory Inventory { get; set; }
            public IEnumerable<Inventory> Inventories { get; set; }
            
        
    }
    
}