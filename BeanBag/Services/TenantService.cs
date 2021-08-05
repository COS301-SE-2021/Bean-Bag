using System;
using System.Linq;
using BeanBag.Database;

namespace BeanBag.Services
{
    public class TenantService : ITenantService
    {
        private readonly TenantDbContext _tenantDb;
        
        public TenantService(TenantDbContext context)
        {
            _tenantDb = context;
        }

        //Get tenant id that the current user belongs to
        public string GetUserTenant(string userId)
        {
            if (userId == null)
            {
                throw new Exception("User object id is null.");
            }
            
            // search user in db
            if (_tenantDb.TenantUser.Find(userId) == null)
            {
                throw new Exception("User does not exist in the database.");
            }
            
            // user is found in db - get result
            var tenantId = (from item
                    in _tenantDb.TenantUser
                where item.UserObjectId.Equals(userId)
                select item.UserTenantId).Single();

            if (tenantId == null)
            {
                throw new Exception("Tenant is null.");
            }
            
            return tenantId;
        }
        
        //Get the name of the current tenant
        public string GetTenantName(string userTenantId)
        {
            if (userTenantId == null)
            {
                throw new Exception("User tenant id is null.");
            }

            if (_tenantDb.Tenant.Find(userTenantId) == null)
            {
                throw new Exception("Tenant id for user not found.");
            }
            
            var tenantName = (from item
                    in _tenantDb.Tenant
                where item.TenantId.Equals(userTenantId)
                select item.TenantName).Single();

            if (tenantName == null)
            {
                throw new Exception("Tenant is null.");
            }

            return tenantName;
        }
        
        //Get the chosen theme for current tenant from database
        public string GetTenantTheme(string userId)
        {
            var userTenantId = GetUserTenant(userId);

            if (userTenantId == null)
            {
                throw new Exception("Tenant is null");
            }
            
            var theme = (from item
                    in _tenantDb.Tenant
                where item.TenantId.Equals(userTenantId)
                select item.TenantTheme).Single();


            if (theme == null)
            {
                throw new Exception("Theme for tenant is null");
            }

            return theme;

        }

    }
}