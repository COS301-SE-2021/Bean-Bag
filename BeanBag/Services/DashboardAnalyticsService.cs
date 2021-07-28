using System;
using System.Linq;
using BeanBag.Database;
namespace BeanBag.Services
{
    public class DashboardAnalyticsService :IDashboardAnalyticsService
    {
        private readonly DBContext _db;
        private readonly IInventoryService _inv;
        
        //Constructor
        public DashboardAnalyticsService( IInventoryService inv, DBContext db)
        {
            _db = db;
            _inv = inv;
        }

        //Gets the recent items added to a specific inventory id in the functions parameter 
        public IOrderedQueryable GetRecentItems(string id)
        {
            var idd =  new Guid(id);
            var result = from i in _db.Items where i.inventoryId.Equals(idd) select new { i.name, i.type, i.imageURL, i.QRContents, i.price, i.entryDate , i.quantity};
            var res= result.OrderByDescending(d => d.entryDate);
            return res;
        }
        
        //Gets the total items added by the user given the user id in the functions parameter 
        public int GetTotalItems(string userId)
        {
            var inventories = _inv.GetInventories(userId);

            var total = 0;
            foreach (var inventory in inventories)
            {
                var res = (from i in _db.Items where i.inventoryId.Equals(inventory.Id) select new {i.quantity}).ToList();
                for (int i = 0; i < res.Count; i++)
                {
                    total += res[i].quantity;
                }
            }
            return total;
            
        }
        

    }
}