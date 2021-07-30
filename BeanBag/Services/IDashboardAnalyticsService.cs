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
        public int GetItemsAvailable(string id);
        public int GetItemsSold(string id);

    }
}