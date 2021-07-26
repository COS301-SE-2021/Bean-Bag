namespace BeanBag.Services
{
    public interface IDashboardAnalyticsService
    {
        public string GetItemName(int position);
        public string GetItemType(int position);
        public string GetItemPrice(int position);
        public string GetItemDate(int position);
        public string GetItemQrCode(int position);


    }
}