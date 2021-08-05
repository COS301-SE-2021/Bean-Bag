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

        //Get tenant that the user belongs to
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
            var tenant = (from item
                    in _tenantDb.TenantUser
                where item.UserObjectId.Equals(userId)
                select item.UserTenantId).Single();

            if (tenant == null)
            {
                throw new Exception("Tenant is null.");
            }
            
            var tenantName = GetTenantName(tenant);

            return tenantName;
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
    }
}