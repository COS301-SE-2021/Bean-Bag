using X.PagedList;

namespace BeanBag.Models
{
    //This class is used to return a pagination model of the database inventory items and models
    public class Pagination
    {
        //Models
        public Inventory Inventory { get; set; }
        
        public Item Item { get; set; }
        public AIModel AiModel { get; set; }
        
        public Tenant Tenant { get; set; }
        
        public TenantUser TenantUser { get; set; }
        
        //Paged list of models
        public IPagedList<Inventory> PagedList{ get; set; }
        public IPagedList<Item> PagedListItems{ get; set; }
        public IPagedList<AIModel> PagedListModels{ get; set; }
        public IPagedList<AIModelVersions> PagedListVersions{ get; set; }
        public IPagedList<Tenant> PagedListTenants{ get; set; }
        public IPagedList<TenantUser> PagedListTenantUsers { get; set; }
        
        public IPagedList<TenantUser> PagedListTenantTransactions { get; set; }
    }
}