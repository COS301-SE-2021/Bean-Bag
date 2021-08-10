using System;
using System.Linq;
using BeanBag.Database;
using BeanBag.Models;

namespace BeanBag.Services
{
    public class TenantService : ITenantService
    {
        private readonly TenantDbContext _tenantDb;
        private string _newTenantId;
        
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

        public bool SetTenantTheme(string userId, string theme)
        {
            if (userId == null)
            {
                throw new Exception("User id is null");
            }

            if (theme == null) return false;

            var tenantId = GetUserTenant(userId);
            var tenant = _tenantDb.Tenant.Find(tenantId);

            if (tenant == null)
            {
                throw new Exception("Tenant id is null");
            }

            tenant.TenantTheme = theme;
            return true;

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

        //Sign new user up under specified tenant
        public bool SignUserUp(string userId, string tenantId)
        {
            if (userId == null || tenantId == null)
            {
                throw new Exception("User or tenant id is null");
            }

            //User already exists
            if (_tenantDb.TenantUser.Find(userId) != null) return false;
            
            //Specified tenant does not exist
            if (_tenantDb.Tenant.Find(tenantId) == null) return false;
            
            //Create new user to add 
            var newUser = new TenantUser {UserObjectId = userId, UserTenantId = tenantId};
            _tenantDb.Add(newUser);
            _tenantDb.SaveChanges();

            return true;

        }
        
        //Create new tenant to add to Tenant database
        public bool CreateNewTenant(string tenantName)
        {
            if (tenantName == null)
            {
                throw new Exception("Tenant name is null");
            }
            
            _newTenantId = new Guid().ToString();

            if (_tenantDb.Tenant.Find(_newTenantId) != null) return false;

            //Create new tenant and add to db
            var newTenant = new Tenant {TenantId = _newTenantId, TenantName = tenantName, TenantTheme = "Blue"};

            _tenantDb.Tenant.Add(newTenant);
            _tenantDb.SaveChanges();
            
            return true;

        }

        public bool SearchTenant(string tenantId)
        {
            if (tenantId == null)
            {
                throw new Exception("Tenant is null");
            }

            return _tenantDb.Tenant.Find(tenantId) != null;
        }

        public bool SearchUser(string userId)
        {
            if (userId == null)
            {
                throw new Exception("User is null");
            }

            return _tenantDb.TenantUser.Find(userId) != null;
        }
    }
}