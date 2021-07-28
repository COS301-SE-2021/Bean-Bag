using System;
using System.Linq;
using BeanBag.Database;

namespace BeanBag.Services
{
    public class DashboardAnalyticsService :IDashboardAnalyticsService
    {
        private readonly DBContext _db;

        public DashboardAnalyticsService(DBContext db)
        {
            _db = db;
        }

        public IOrderedQueryable GetRecentItems(string id)
        {
            var idd =  new Guid(id);
            var result = from i in _db.Items where i.inventoryId.Equals(idd) select new { i.name, i.type, i.imageURL, i.QRContents, i.price, i.entryDate };
            var res= result.OrderByDescending(d => d.entryDate);
            return res;
        }


    }
}