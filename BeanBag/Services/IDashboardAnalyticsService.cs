using System.Linq;

namespace BeanBag.Services
{
    public interface IDashboardAnalyticsService
    {
        public IOrderedQueryable GetRecentItems(string id);
        public int GetTotalItems(string id);
        public IQueryable GetTopItems(string id);
        public int GetItemsAvailable(string id, string time);
        public int GetItemsSold(string id,string time);
        public double GetRevenue(string id, string time);
       double GetSalesGrowth(string id, string time) ;
    }
}