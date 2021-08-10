using X.PagedList;

namespace BeanBag.Models
{
    //This class is used to return a pagination model of the database inventory items
    public class Pagination
    {
        public Inventory Inventory { get; set; }
        public IPagedList<Inventory> PagedList{ get; set; }

    }
    
}