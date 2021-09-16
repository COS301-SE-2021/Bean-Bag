using System;
using System.Collections.Generic;
using System.Linq;
using BeanBag.Database;
using BeanBag.Models;

namespace BeanBag.Services
{
    /* This service implements all the Tenant interface functions and provides interaction
     with the Tenant database */
    
    public class TenantService : ITenantService
    {
        private readonly TenantDbContext _tenantDb;
        private string _newTenantId;

        //Constructor sets database context
        public TenantService(TenantDbContext context)
        {
            _tenantDb = context;
        }

        //Tenant functions
        public string GetUserTenantId(string userId)
        {
            if (userId == null)
            {
                throw new Exception("User object id is null.");
            }
            
            //Search for user in database
            if (_tenantDb.TenantUser.Find(userId) == null)
            {
                throw new Exception("User does not exist in the database.");
            }
            
            //User found
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
        
        /*Retrieves the name of the tenant from the database - based on the
         id of the tenant*/
        public string GetTenantName(string userTenantId)
        {
            if (userTenantId == null)
            {
                throw new Exception("User tenant id is null.");
            }

            //Search tenant
            if (_tenantDb.Tenant.Find(userTenantId) == null)
            {
                throw new Exception("Tenant id for user not found.");
            }
            
            //Tenant found
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
        
        /* Retrieves the name of the tenant from the database - based on the tenant name */
        public string GetTenantId(string tenantName)
        {
            if (tenantName == null)
            {
                throw new Exception("Tenant name is null");
            }

            var tenantId = (from tenant
                    in _tenantDb.Tenant
                where tenant.TenantName.Equals(tenantName)
                select tenant.TenantId).Single();

            return tenantId;
        }
        
        /* Retrieve the current tenant from the user id */
        public Tenant GetCurrentTenant(string userId)
        {
            if (userId == null)
            {
                throw new Exception("User id is null");
            }

            var tenantId = GetUserTenantId(userId);

            var tenant = _tenantDb.Tenant.Find(tenantId);

            return tenant;
        }
        
        /* Sets the theme of the tenant */
        public bool SetTenantTheme(string userId, string theme)
        {
            if (userId == null)
            {
                throw new Exception("User id is null");
            }

            if (theme == null) return false;

            var tenantId = GetUserTenantId(userId);
            var tenant = _tenantDb.Tenant.Find(tenantId);

            if (tenant == null)
            {
                throw new Exception("Tenant id is null");
            }

            tenant.TenantTheme = theme;
            _tenantDb.SaveChanges();
            
            return true;

        }
        
        /* Retrieves the theme of the tenant to load */
        public string GetTenantTheme(string userId)
        {
            // return default when user is not signed in - Layout
            if (userId == null)
            {
                throw new Exception("User id is null");
            }

            if (SearchUser(userId) == false)
            {
                return "Default";
            }
            

            var userTenantId = GetUserTenantId(userId);

            if (userTenantId == null)
            {
                throw new Exception("Tenant is null");
            }
            
            var theme = (from item
                    in _tenantDb.Tenant
                where item.TenantId.Equals(userTenantId)
                select item.TenantTheme).Single() ?? "Default";


            return theme;

        }

        /* Retrieves a list of all existing tenants in the database */
        public IEnumerable<Tenant> GetTenantList()
        {
            var tenants = from tenant
                    in _tenantDb.Tenant
                select tenant;

            var list = tenants.ToList();

            return list;
        }
        
        /* Creates a new tenant and adds tenant to the database */
        public bool CreateNewTenant(string tenantName, string tenantAddress, string tenantEmail, string tenantNumber, string tenantSubscription)
        {
            if (tenantName == null)
            {
                throw new Exception("Tenant name is null");
            }

            if (tenantEmail == null)
            {
                throw new Exception("Tenant email is null");
            }

            if (tenantAddress == null)
            {
                throw new Exception("Tenant address is null");
            }

            if (tenantNumber == null)
            {
                throw new Exception("Tenant number is null");
            }

            if (tenantSubscription == null)
            {
                throw new Exception("Tenant subscription is null");
            }
            var duplicate = (from tenant
                    in _tenantDb.Tenant
                where tenant.TenantName.Equals(tenantName)
                select tenant.TenantName).FirstOrDefault();

            if (duplicate != null)
            {
                return false;
            }

            var id = Guid.NewGuid();
            
            _newTenantId = id.ToString();

            if (_tenantDb.Tenant.Find(_newTenantId) != null) return false;

            
            const string defaultTheme = "Default";
            const string defaultLogo = "/images/beanbaglogo.png";

            var code = GenerateCode();

            //Create new tenant and add to db
            var newTenant = new Tenant {TenantId = _newTenantId, TenantName = tenantName, TenantTheme = defaultTheme, 
                TenantEmail = tenantEmail, TenantAddress = tenantAddress, TenantNumber = tenantNumber, TenantSubscription = tenantSubscription,
                TenantLogo = defaultLogo, InviteCode = code
            };

            _tenantDb.Tenant.Add(newTenant);
            _tenantDb.SaveChanges();
            
            return true;

        }

        public void EditTenantDetails(string tenantId, string tenantName, string address, string email, string number)
        {
            var tenant = _tenantDb.Tenant.Find(tenantId);

            if (tenant == null)
            {
                throw new Exception("Tenant is null");
            }

            tenant.TenantName = tenantName;
            tenant.TenantAddress = address;
            tenant.TenantEmail = email;
            tenant.TenantNumber = number;

            _tenantDb.SaveChanges();
        }

        /* Searches database for tenant by tenant id */
        public bool SearchTenant(string tenantId)
        {
            if (tenantId == null)
            {
                throw new Exception("Tenant is null");
            }

            return _tenantDb.Tenant.Find(tenantId) != null;
        }

        /* Sets the logo for the tenant */
        public void SetLogo(string userId, string logo)
        {
            if (userId == null)
            {
                throw new Exception("User Id is null");
            }

            if (logo == null)
            {
                throw new Exception("Logo is null");
            }
            
            var tenantId = GetUserTenantId(userId);

            var tenant = _tenantDb.Tenant.Find(tenantId);

            tenant.TenantLogo = logo;
            _tenantDb.SaveChanges();
            
        }

        /* Retrieves the tenant logo */
        public string GetLogo(string userId)
        {
            if (userId == null)
            {
                throw new Exception("User id is null");
            }

            if (SearchUser(userId) == false)
            {
                return "LOGO HERE";
            }

            var tenantId = GetUserTenantId(userId);

            if (tenantId == null)
            {
                throw new Exception("Tenant id is null");
            }

            var tenant = _tenantDb.Tenant.Find(tenantId);
            return tenant.TenantLogo;
        }

        /* Generate a random GUID code for the tenant */
        public string GenerateCode()
        {
            var code = Guid.NewGuid().ToString();

            return code;
        }

        public string GetTenantCode(string tenantId)
        {
            if (tenantId == null)
            {
                throw new Exception("Tenant id is null.");
            }

            var tenant = _tenantDb.Tenant.Find(tenantId);

            if (tenant == null)
            {
                throw new Exception("Tenant is null.");
            }

            var code = tenant.InviteCode;

            if (code == null)
            {
                throw new Exception("Code is null.");
            }

            return code;
        }
        
        /* Verify if user entered valid invite code */
        public bool VerifyCode(string code)
        {
            if (code == null)
            {
                throw new Exception("Entered code is null.");
            }

            var check = (from tenant
                    in _tenantDb.Tenant
                where tenant.InviteCode.Equals(code)
                select tenant).FirstOrDefault();

            if (check == null)
            {
                return false;
            }

            if (check.InviteCode.Equals(code))
            {
                return true;
            }

            return false;
        }
        
        /* Returns the tenant the invite belongs to */
        public Tenant GetInvitationTenant(string code)
        {
            if (code == null)
            {
                throw new Exception("Code is null");
            }

            var tenant = (from t
                    in _tenantDb.Tenant
                where t.InviteCode.Equals(code)
                select t).Single();

            if (tenant == null)
            {
                throw new Exception("Tenant is null");
            }

            return tenant;

        }
        
        //User functions
        /* Signs the user up and adds user to the user table in the database */
        public bool SignUserUp(string userId, string tenantId, string userName)
        {
            if (userId == null || tenantId == null)
            {
                throw new Exception("User or tenant id is null");
            }

            //User already exists
            if (_tenantDb.TenantUser.Find(userId) != null) return false;
            
            //Specified tenant does not exist
            if (_tenantDb.Tenant.Find(tenantId) == null) return false;

            //Create new user
            var newUser = new TenantUser {UserObjectId = userId, UserTenantId = tenantId, UserName = userName, UserRole = "U"};
            _tenantDb.Add(newUser);
            _tenantDb.SaveChanges();

            return true;

        }

        /* Searches for user in the database by user id */
        public bool SearchUser(string userId)
        {
            if (userId == null)
            {
                throw new Exception("User is null");
            }

            return _tenantDb.TenantUser.Find(userId) != null;
        }
        
        /* Returns a list of the users belonging to a specific tenant */
        public IEnumerable<TenantUser> GetUserList(string userId)
        {
            if (userId == null)
            {
                throw new Exception("User id is null");
            }

            var tenantId = GetUserTenantId(userId);

            if (tenantId == null)
            {
                throw new Exception("Tenant id is null");
            }

            var users = from user
                    in _tenantDb.TenantUser
                where user.UserTenantId.Equals(tenantId)
                select user;

            var userList = users.ToList();

            return userList;

        }

        /* Delete user from the tenant database */
        public bool DeleteUser(string userId)
        {
            if (userId == null)
            {
                throw new Exception("User id is null");
            }

            var user = _tenantDb.TenantUser.Find(userId);

            if (user == null)
            {
                throw new Exception("User is null");
            }

            _tenantDb.TenantUser.Remove(user);
            _tenantDb.SaveChanges();
            
            var check =  _tenantDb.TenantUser.Find(userId);

            if (check == null)
            {
                return true;
            }

            return false;
        }
        
        // This method retrieves the user role from the database
        public string GetUserRole(string id)
        {
            var user = _tenantDb.TenantUser.Find(id);

            if (user == null)
            {
                throw new Exception("User is null");
            }

            if (user.UserRole.Equals("U"))
            {
                return "User";
            }
            
            if (user.UserRole.Equals("A"))
            {
                return "Admin";
            }

            return "";
        }

    

        // This method updated the role of the user
        public bool EditUserRole(string userId, string role)
        {
            if (userId == null)
            {
                throw new Exception("User id is null");
            }
        
            if (role == null)
            {
                throw new Exception("User role is null");
            }

            var user = _tenantDb.TenantUser.Find(userId);
        
            if (role.Equals("Admin"))
            {
                user.UserRole = "A";
                _tenantDb.SaveChanges();
                return true;
            }
                    
            if (role.Equals("User"))
            {
                user.UserRole = "U";
                _tenantDb.SaveChanges();
                return true;
            }
        
            return false;
        }
        
        //This method delete the current tenant and all users
        public void DeleteTenant(string userId)
        {
            var currentTenant = GetCurrentTenant(userId);
            var tenantId = currentTenant.TenantId;

            if (currentTenant == null)
            {
                throw new Exception("Tenant is null.");
            }

            //Delete all the users under the tenant
            var deleted = from user
                    in _tenantDb.TenantUser
                where user.UserTenantId.Equals(tenantId)
                select user;

            foreach (var user in deleted)
            {
                _tenantDb.TenantUser.Remove(user);
                _tenantDb.SaveChanges();
            }
            
            //Delete tenant
            var tenant = _tenantDb.Tenant.Find(tenantId);

            if (tenant == null)
            {
                throw new Exception("Tenant is null.");
            }

            _tenantDb.Tenant.Remove(tenant);
            _tenantDb.SaveChanges();

        }
        // This method updates the subscription plan of the tenant
        // This function sets the updated subscription for the user
        // This is only for a free tenant subscription update not paid
        public void UpdateSubscription(string subscription, string tenantId)
        {
            if (subscription == null)
            {
                throw new Exception("Subscription is null");
            }else if (tenantId == null)
            {
                throw new Exception("TenantID is null");
            }
            
            if (subscription == "Free")
            {
                var tenant = _tenantDb.Tenant.Find(tenantId);

                if (tenant == null)
                {
                    throw new Exception("Tenant id not found");
                }
                tenant.TenantSubscription = "Free";
                _tenantDb.SaveChanges();
            }
            else
            {
                throw new Exception("Incorrect subscription input.");
            }
        }
    }
}