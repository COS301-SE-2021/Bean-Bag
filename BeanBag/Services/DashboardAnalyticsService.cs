using BeanBag.Database;

namespace BeanBag.Services
{
    public class DashboardAnalyticsService
    {
        private readonly DBContext _db;

        public DashboardAnalyticsService(DBContext db)
        {
            _db = db;
        }
    }
}