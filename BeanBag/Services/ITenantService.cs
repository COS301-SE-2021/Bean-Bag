using System;
using BeanBag.Models;

namespace BeanBag.Services
{
    public interface ITenantService
    {
        public string GetTenantName(string tenantId);

        public string GetUserTenantId(string userId);

        public string GetTenantTheme(string userId);

        public bool SetTenantTheme(string userId, string theme);
        
        public bool CreateNewTenant(string tenantName, string theme);

        public bool SearchTenant(string tenantId);

        public bool SignUserUp(string userId, string tenantId);

        public bool SearchUser(string userId);

    }
}