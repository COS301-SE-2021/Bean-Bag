using System.Collections.Generic;
using BeanBag.Models;

namespace BeanBag.Services
{
    public interface ITenantService
    {
        public string GetTenantName(string tenantId);
        public string GetUserTenantId(string userId);
        public string GetTenantId(string tenantName);
        public string GetTenantTheme(string userId);
        public bool SetTenantTheme(string userId, string theme);
        public bool CreateNewTenant(string tenantName);
        public bool SearchTenant(string tenantId);
        public IEnumerable<Tenant> GetTenantList();
        public void SetLogo(string userId, string logo);
        public string GetLogo(string userId);
        public bool SignUserUp(string userId, string tenantId);
        public bool SearchUser(string userId);

    }
}