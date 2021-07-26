using BeanBag.Database;
using BeanBag.Models;

namespace BeanBag.Services
{
    public class DashboardAnalyticsService
    {
        private readonly DBContext _db;
        private readonly Item _item;
        private readonly Inventory _inventory;

        public DashboardAnalyticsService(DBContext db)
        {
            _db = db;
        }

        public string GetItemName(int position)
        {
            return "";
        }

        public string GetItemType(int position)
        {
            return "";
        }

        public string GetItemPrice(int position)
        {
            return "";
        }

        public string GetItemDate(int position)
        {
            return "";
        }

        public string GetItemQrCode(int position)
        {
            return "";
        }
        

    }
}