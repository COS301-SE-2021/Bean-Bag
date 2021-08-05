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
        public string GetUserTenant(string tenantId)
        {
            if (tenantId == null)
            {
                
            }
            
            // search user in db
            if (_tenantDb.TenantUser.Find(tenantId) == null)
            {
                
            }
            
            // user is found in db - get result
            var tenant = (from item
                    in _tenantDb.TenantUser
                where item.UserObjectId.Equals(tenantId)
                select item.UserTenantId).Single();

            return GetTenantName(tenant);
        }
        
        public string GetTenantName(string userTenantId)
        {
            var tenantName = (from item
                    in _tenantDb.Tenant
                where item.TenantId.Equals(userTenantId)
                select item.TenantName).Single();

            return tenantName;
        }
    }
}