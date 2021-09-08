using System.Collections.Generic;
using BeanBag.Models;

namespace BeanBag.Services
{
    // This class is an interface for the tenant service.
    public interface ITenantService
    {
        public string GetTenantName(string tenantId);
        public string GetUserTenantId(string userId);
        public string GetTenantId(string tenantName);
        public Tenant GetCurrentTenant(string userId);
        public string GetTenantTheme(string userId);
        public bool SetTenantTheme(string userId, string theme);
        public bool CreateNewTenant(string tenantName, string address, string email, string number, string subscription);
        public void EditTenantDetails(string tenantId, string tenantName, string address, string email, string number);
        public bool SearchTenant(string tenantId);
        public IEnumerable<Tenant> GetTenantList();
        public void SetLogo(string userId, string logo);
        public string GetLogo(string userId);
        public string GenerateCode();
        public bool SignUserUp(string userId, string tenantId, string userName);
        public bool SearchUser(string userId);
        public IEnumerable<TenantUser> GetUserList(string userId);
        public bool DeleteUser(string userId);
        public bool EditUserRole(string userId, string role);
        public string GetUserRole(string id);

    }
}