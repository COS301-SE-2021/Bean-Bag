using System.Linq;

namespace BeanBag.Services
{
    // This class is an interface for the Dashboard Analytics service.
    public interface IDashboardAnalyticsService
    {
        public IOrderedQueryable GetRecentItems(string id);
        public int GetTotalItems(string id);
        public IQueryable GetTopItems(string id);
        public int GetItemsAvailable(string id, string time);
        public int GetItemsSold(string id,string time);
        public double GetRevenue(string id, string time);
       double GetSalesGrowth(string id, string time) ;
       double ItemsRevenueStat(string id, string time);
       double ItemsSoldStat(string id, string time);
       double ItemAvailableStat(string id, string time);
    }
}