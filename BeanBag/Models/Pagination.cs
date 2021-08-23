using X.PagedList;

namespace BeanBag.Models
{
    //This class is used to return a pagination model of the database inventory items and models
    public class Pagination
    {
        public Inventory Inventory { get; set; }
        public Item Item { get; set; }
        public AIModel AIModel { get; set; }
        public AIModelVersions AIModelVersions { get; set; }
        public IPagedList<Inventory> PagedList{ get; set; }
        public IPagedList<Item> PagedListItems{ get; set; }
        public IPagedList<AIModel> PagedListModels{ get; set; }
        public IPagedList<AIModelVersions> PagedListVersions{ get; set; }
    }
}