using System;
using System.Collections.Generic;
using System.Linq;

namespace BeanBag.Services
{
    public interface IDashboardAnalyticsService
    {
        public IOrderedQueryable GetRecentItems(string id);
        public int GetTotalItems(string id);
        public IQueryable GetTopItems(string id);
        public int GetItemsAvailable(string id, string time);
        public int GetItemsSold(string id);
        public double GetRevenue(string id);
       double GetSalesGrowth(string id);
    }
}