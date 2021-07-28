using System;
using System.Linq;
using BeanBag.Database;
namespace BeanBag.Services
{
    public class DashboardAnalyticsService :IDashboardAnalyticsService
    {
        private readonly DBContext _db;
        
        //Constructor
        public DashboardAnalyticsService( DBContext db)
        {
            _db = db;
        }

        //Gets the recent items added to a specific inventory id in the functions parameter 
        public IOrderedQueryable GetRecentItems(string id)
        {
            var idd =  new Guid(id);
            var result = from i in _db.Items where i.inventoryId.Equals(idd) select new { i.name, i.type, i.imageURL, i.QRContents, i.price, i.entryDate , i.quantity};
            var res= result.OrderByDescending(d => d.entryDate);
            return res;
        }
        
        //Gets the total items added by the user given the inventory id in the functions parameter 
        public int GetTotalItems(string id)
        {
            var idd =  new Guid(id);
            var res = (from i in _db.Items where i.inventoryId.Equals(idd) select new {i.quantity}).ToList();
            return res.Sum(t => t.quantity);
            
        }
        

    }
}