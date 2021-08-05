using System.Linq;

namespace BeanBag.Services
{
    public interface ITenantService
    {
        public string GetTenantName(string tenantId);

        public string GetUserTenant(string userId);

        public string GetTenantTheme(string userId);

    }
}