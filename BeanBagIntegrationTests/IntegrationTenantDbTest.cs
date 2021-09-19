using BeanBag.Database;
using BeanBag.Models;
using BeanBag.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using Microsoft.Extensions.Configuration;
using Xunit;

namespace BeanBagIntegrationTests
{
    public class IntegrationTenantDbTest
    {
        private readonly TenantDbContext _tenantDbContext;
        
        private readonly IConfiguration _configuration;

        // Tenant testing data
        private const string Name = "Tenant-name-testing";
        private const string Address = "test-address";
        private const string Email = "tenant@test.com";
        private const string Number = "0123456789";
        private const string Subscription = "Free";
        private const string Theme = "Default";
        private const string InviteCode = "";
        
        private const string Name2 = "Tenant-name-2-testing";
        private const string Address2 = "test-address-2";
        private const string Email2 = "tenant2@test.com";
        private const string Number2 = "0123456789";
        private const string Subscription2 = "Free";

        // User testing data
        private const string Username = "test-user";
        private const string Role = "U";

        
        
        public IntegrationTenantDbTest()
        {
            this._configuration = new ConfigurationBuilder().AddJsonFile("appsettings.local.json").Build();
            
            var serviceProvider = new ServiceCollection()
                .AddEntityFrameworkSqlServer()
                .BuildServiceProvider();

            var builder = new DbContextOptionsBuilder<TenantDbContext>();

            var connString = _configuration.GetValue<string>("Database:TenantConnection");
            
            builder.UseSqlServer(connString).UseInternalServiceProvider(serviceProvider);

            _tenantDbContext = new TenantDbContext(builder.Options);
        }
        
        //Test tenant creation
        //POSITIVE TEST
        [Fact]
        public void Test_Tenant_Creation_Success_Tenant_Added()
        {
            //Arrange
            var query = new TenantService(_tenantDbContext);
            
            //Act
            var created = query.CreateNewTenant(Name,Address,Email,Number,Subscription);
            var tenantId = query.GetTenantId(Name);


            //Assert
            Assert.Equal(created, tenantId);
            
            //Delete from the database
            _tenantDbContext.Tenant.Remove(_tenantDbContext.Tenant.Find(tenantId));
            _tenantDbContext.SaveChanges();

        }
        
        //NEGATIVE TEST
        [Fact]
        public void Test_Tenant_Creation_Fail_Tenant_Name_Is_Null()
        {
            //Arrange
            var query = new TenantService(_tenantDbContext);

            //Act
            var exception = Assert.Throws<Exception>(() => query.CreateNewTenant(null,Address,Email,Number,Subscription));

            //Assert
            Assert.Equal("Tenant name is null", exception.Message);
        }
        
        [Fact]
        public void Test_Tenant_Creation_Fail_Tenant_Email_Is_Null()
        {
            //Arrange
            var query = new TenantService(_tenantDbContext);

            //Act
            var exception = Assert.Throws<Exception>(() => query.CreateNewTenant(Name,Address,null,Number,Subscription));

            //Assert
            Assert.Equal("Tenant email is null", exception.Message);
        }
        
        [Fact]
        public void Test_Tenant_Creation_Fail_Tenant_Address_Is_Null()
        {
            //Arrange
            var query = new TenantService(_tenantDbContext);

            //Act
            var exception = Assert.Throws<Exception>(() => query.CreateNewTenant(Name,null,Email,Number,Subscription));

            //Assert
            Assert.Equal("Tenant address is null", exception.Message);
        }
        
        [Fact]
        public void Test_Tenant_Creation_Fail_Tenant_Number_Is_Null()
        {
            //Arrange
            var query = new TenantService(_tenantDbContext);

            //Act
            var exception = Assert.Throws<Exception>(() => query.CreateNewTenant(Name,Address,Email,null,Subscription));

            //Assert
            Assert.Equal("Tenant number is null", exception.Message);
        }
        
        [Fact]
        public void Test_Tenant_Creation_Fail_Tenant_Subscription_Is_Null()
        {
            //Arrange
            var query = new TenantService(_tenantDbContext);

            //Act
            var exception = Assert.Throws<Exception>(() => query.CreateNewTenant(Name,Address,Email,Number,null));

            //Assert
            Assert.Equal("Tenant subscription is null", exception.Message);
        }
        
        /*
        //POSITIVE TESTING
        [Fact]
        public void Test_Get_Current_Tenant()
        {
            //Arrange
            var query = new TenantService(_tenantDbContext);
            
            //Act
            var user = "95d8316b-838a-418c-9f48-473b1790c63d";
            
            query.CreateNewTenant(Name,Address,Email,Number,Subscription);
            var tenantId = query.GetTenantId(Name);

            query.SignUserUp(user, tenantId, "test-user-integration");

            var iTen = query.GetCurrentTenant(user);
            
            //Assert
            Assert.Equal(iTen.TenantName, Name);
            
            //Delete from the database
            _tenantDbContext.Tenant.Remove(_tenantDbContext.Tenant.Find(tenantId));
            _tenantDbContext.SaveChanges();
            query.DeleteUser(user);

        }
        */
        
        
        //Test to get a list of tenants from the database
        //POSITIVE TEST
        [Fact]
        public void Test_Get_Tenants_From_Database_Success_Tenants_Added()
        {
            //Arrange
            var tenantId1 = Guid.NewGuid().ToString();
            var tenant1 = new Tenant
            {
                TenantId = tenantId1, TenantName = Name, TenantAddress = Address, 
                TenantEmail = Email, TenantNumber = Number, TenantSubscription = Subscription, InviteCode = "a", TenantLogo = "", TenantTheme = ""
            };
            
            var tenantId2 = Guid.NewGuid().ToString();
            var tenant2 = new Tenant
            {
                TenantId = tenantId2, TenantName = Name2, TenantAddress = Address2, 
                TenantEmail = Email2, TenantNumber = Number2, TenantSubscription = Subscription2, InviteCode = "", TenantLogo = "", TenantTheme = ""
            };

            //Act
            var queryExisting = new TenantService(_tenantDbContext);
            var existing = queryExisting.GetTenantList().Count();
            
            _tenantDbContext.Tenant.Add(tenant1);
            _tenantDbContext.SaveChanges();
            _tenantDbContext.Tenant.Add(tenant2);
            _tenantDbContext.SaveChanges();
            
            var query = new TenantService(_tenantDbContext);
            var tenantList = query.GetTenantList();
            
            var tenant = _tenantDbContext.Tenant.Find(tenantId1);
            
            //Assert
            Assert.NotNull(tenantList);
            Assert.Equal(2+existing,tenantList.Count());
            Assert.Equal(tenantId1,tenant.TenantId);
            Assert.Equal(Name, tenant.TenantName);

            //Delete tenants from database
            _tenantDbContext.Tenant.Remove(_tenantDbContext.Tenant.Find(tenantId1));
            _tenantDbContext.SaveChanges();
            _tenantDbContext.Tenant.Remove(_tenantDbContext.Tenant.Find(tenantId2));
            _tenantDbContext.SaveChanges();
        }
        
        //NEGATIVE TEST
        [Fact]
        public void Test_Get_Tenants_From_Database_Fail_Tenants_Not_Added()
        {
            //Arrange
            var tenantId1 = Guid.NewGuid();

            var query = new TenantService(_tenantDbContext);
            var tenantList = query.GetTenantList();

            //Act
            var queryExisting = new TenantService(_tenantDbContext);
            var existing = queryExisting.GetTenantList().Count();

            var tenant = _tenantDbContext.Tenant.Find(tenantId1.ToString());
            
            //Assert
            Assert.Null(tenant);
            Assert.NotEqual(2+existing,tenantList.Count());
            
        }
        
        
        //Get tenant id from current user
        //POSITIVE TEST
      /*  [Fact]
        public void Test_Get_Tenant_Id_Success_Tenant_Exists()
        {
            //Arrange
            var id = Guid.NewGuid().ToString();
            var newTenant = new Tenant
            {
                TenantId = id, TenantName = Name, TenantAddress = Address, 
                TenantEmail = Email, TenantNumber = Number, TenantSubscription = Subscription
            };

            //Act
            var query = new TenantService(_tenantDbContext);
            _tenantDbContext.Tenant.Add(newTenant);
            _tenantDbContext.SaveChanges();

            var tenantId = query.GetTenantId(Name);

            var tenant = _tenantDbContext.Tenant.Find(newTenant.TenantId);

            //Assert
            Assert.NotNull(tenant);
            Assert.NotNull(tenantId);
            Assert.Equal(id, tenantId);
            Assert.Equal(Name, tenant.TenantName);

            //Delete from database
            _tenantDbContext.Remove(tenant);
            _tenantDbContext.SaveChanges();
        }*/
        
        //NEGATIVE TEST
        [Fact]
        public void Test_Get_Tenant_Id_Failure_Tenant_Does_Not_Exist()
        {
            //Arrange
            var id = Guid.NewGuid().ToString();
            var newTenant = new Tenant
            {
                TenantId = id, TenantName = Name, TenantAddress = Address, 
                TenantEmail = Email, TenantNumber = Number, TenantSubscription = Subscription
            };

            //Act
            var query = new TenantService(_tenantDbContext);
            var tenant = _tenantDbContext.Tenant.Find(newTenant.TenantId);
            var exception = Assert.Throws<Exception>(() => query.GetTenantId(null));

            //Assert
            Assert.Null(tenant);
            Assert.Equal("Tenant name is null", exception.Message);
        }
        
        
        //Get name of the tenant
        //POSITIVE TEST
        [Fact]
        public void Test_Get_Tenant_Name_Success_Tenant_Exists()
        {
            //Arrange
            //Tenant
            var id = Guid.NewGuid().ToString();
            var newTenant = new Tenant
            {
                TenantId = id, TenantName = Name, TenantAddress = Address,
                TenantEmail = Email, TenantNumber = Number, TenantSubscription = Subscription, InviteCode = "", TenantLogo = "", TenantTheme = ""
            };
            
            //User
            var userId = Guid.NewGuid().ToString();
            var newUser = new TenantUser
            {
                UserTenantId = id, UserObjectId = userId,
                UserName = Username, UserRole = Role
                
            };

            //Act
            var query = new TenantService(_tenantDbContext);
            _tenantDbContext.Tenant.Add(newTenant);
            _tenantDbContext.SaveChanges();
            _tenantDbContext.TenantUser.Add(newUser);
            _tenantDbContext.SaveChanges();

            var tenant = _tenantDbContext.Tenant.Find(id);
            var user = _tenantDbContext.TenantUser.Find(userId);
            var tenantName = query.GetTenantName(id);
            
            //Assert
            Assert.NotNull(tenant);
            Assert.NotNull(tenantName);
            Assert.Equal(Name, tenantName);

            //Delete from database
            _tenantDbContext.Remove(user);
            _tenantDbContext.SaveChanges();
            _tenantDbContext.Remove(tenant);
            _tenantDbContext.SaveChanges();
        }
        
        //NEGATIVE TEST
        [Fact]
        public void Test_Get_Tenant_Name_Fail_Tenant_Is_Null()
        {
            //Arrange
            //Tenant
            var id = Guid.NewGuid().ToString();

            //Act
            var query = new TenantService(_tenantDbContext);

            var tenant = _tenantDbContext.Tenant.Find(id);

            var exceptionId = Assert.Throws<Exception>(() => query.GetTenantName(null));

            //Assert
            Assert.Null(tenant);
            Assert.Equal("User tenant id is null.", exceptionId.Message);

        }
        
        //Tenant search
        //POSITIVE TEST
        [Fact]
        public void Test_Tenant_Search_Success_Tenant_Found()
        {
            //Arrange
            //Tenant
            var id = Guid.NewGuid().ToString();
            var newTenant = new Tenant
            {
                TenantId = id, TenantName = Name, TenantAddress = Address, 
                TenantEmail = Email, TenantNumber = Number, TenantSubscription = Subscription, InviteCode = "", TenantLogo = "", TenantTheme = ""
            };
            
            //Act
            var query = new TenantService(_tenantDbContext);
            _tenantDbContext.Tenant.Add(newTenant);
            _tenantDbContext.SaveChanges();

            var tenant = _tenantDbContext.Tenant.Find(id);
            var searchTenant = query.SearchTenant(id);
            
            //Assert
            Assert.NotNull(tenant);
            Assert.True(searchTenant);
            
            //Delete from database
            _tenantDbContext.Tenant.Remove(tenant);
            _tenantDbContext.SaveChanges();
        }
        
        //NEGATIVE TEST
        [Fact]
        public void Test_Tenant_Search_Fail_Tenant_Not_Found()
        {
            //Arrange
            //Tenant
            var id = Guid.NewGuid().ToString();

            //Act
            var query = new TenantService(_tenantDbContext);
            
            var tenant = _tenantDbContext.Tenant.Find(id);
            var searchTenant = query.SearchTenant(id);
            
            var exception = Assert.Throws<Exception>(() => query.SearchTenant(null));
            
            //Assert
            Assert.Null(tenant);
            Assert.False(searchTenant);
            Assert.Equal("Tenant is null", exception.Message);
        }
        
        
        //Test getting tenant id from current user
        //POSITIVE TEST
        [Fact]
        public void Test_Tenant_Id_Retrieval_Success_Tenant_Found()
        {
            //Arrange
            //Tenant
            var id = Guid.NewGuid().ToString();
            var newTenant = new Tenant
            {
                TenantId = id, TenantName = Name, TenantAddress = Address, 
                TenantEmail = Email, TenantNumber = Number, TenantSubscription = Subscription, InviteCode = "", TenantLogo = "", TenantTheme = ""
            };
            
            //User
            var userId = Guid.NewGuid().ToString();
            var newUser = new TenantUser
            {
                UserTenantId = id, UserObjectId = userId,
                UserName = Username, UserRole = Role
                
            };

            //Act
            var query = new TenantService(_tenantDbContext);
            _tenantDbContext.Tenant.Add(newTenant);
            _tenantDbContext.SaveChanges();
            _tenantDbContext.TenantUser.Add(newUser);
            _tenantDbContext.SaveChanges();

            var tenant = _tenantDbContext.Tenant.Find(id);
            var user = _tenantDbContext.TenantUser.Find(userId);
            var tenantId = query.GetUserTenantId(userId);
            
            //Assert
            Assert.NotNull(tenant);
            Assert.NotNull(user);
            Assert.NotNull(tenantId);

            //Delete from database
            _tenantDbContext.TenantUser.Remove(user);
            _tenantDbContext.SaveChanges();
            _tenantDbContext.Tenant.Remove(tenant);
            _tenantDbContext.SaveChanges();
        }
        
        //NEGATIVE TEST
        [Fact]
        public void Test_Tenant_Id_Retrieval_Fail_User_Is_Null()
        {
            //Arrange
            var query = new TenantService(_tenantDbContext);

            //Act
            var exception = Assert.Throws<Exception>(() => query.GetUserTenantId(null));

            //Assert
            Assert.Equal("User object id is null.", exception.Message);
        }
        
        //NEGATIVE TEST
        [Fact]
        public void Test_Tenant_Id_Retrieval_Fail_User_Does_Not_Exist()
        {
            //Arrange
            var userId = Guid.NewGuid().ToString();
            var query = new TenantService(_tenantDbContext);
            
            //Act
            var exception = Assert.Throws<Exception>(() => query.GetUserTenantId(userId));
            
            //Assert
            Assert.Equal("User does not exist in the database.", exception.Message);

        } 
        
        
        //Test setting tenant theme
        //POSITIVE TEST
        [Fact]
        public void Test_Setting_Theme_Success_Theme_Updated()
        {
            //Arrange
            var id = Guid.NewGuid().ToString();
            var newTenant = new Tenant
            {
                TenantId = id, TenantName = Name, TenantAddress = Address, 
                TenantEmail = Email, TenantNumber = Number, TenantSubscription = Subscription, InviteCode = "", TenantLogo = "", TenantTheme = ""
            };

            var userId = Guid.NewGuid().ToString();
            var newUser = new TenantUser
            {
                UserTenantId = id, UserObjectId = userId,
                UserName = Username, UserRole = Role
                
            };
            
            var query = new TenantService(_tenantDbContext);
            
            //Act
            _tenantDbContext.Tenant.Add(newTenant);
            _tenantDbContext.SaveChanges();
            _tenantDbContext.TenantUser.Add(newUser);
            _tenantDbContext.SaveChanges();

            var updated = query.SetTenantTheme(userId, "Default");

            var tenant = _tenantDbContext.Tenant.Find(id);
            var user = _tenantDbContext.TenantUser.Find(userId);

            //Assert
            Assert.NotNull(tenant);
            Assert.NotNull(user);
            Assert.True(updated);
            Assert.Equal("Default", tenant.TenantTheme);
            
            //Delete from the database
            _tenantDbContext.TenantUser.Remove(user);
            _tenantDbContext.SaveChanges();
            _tenantDbContext.Tenant.Remove(tenant);
            _tenantDbContext.SaveChanges();
        }
        
        //NEGATIVE TEST
        [Fact]
        public void Test_Setting_Theme_Fail_User_Is_Null()
        {
            //Arrange
            var query = new TenantService(_tenantDbContext);
            
            //Act
            var exception = Assert.Throws<Exception>(() => query.SetTenantTheme(null, "Default"));

            //Assert
            Assert.Equal("User id is null", exception.Message);
            
        }
        
        
        //Test getting tenant theme
        //POSITIVE TEST
        [Fact]
        public void Test_Setting_Theme_Success_Theme_Returned()
        {
            //Arrange
            var id = Guid.NewGuid().ToString();
            var newTenant = new Tenant
            {
                TenantId = id, TenantName = Name, TenantAddress = Address, 
                TenantEmail = Email, TenantNumber = Number, TenantSubscription = Subscription, InviteCode = "", TenantLogo = "", TenantTheme = "Default"
            };

            var userId = Guid.NewGuid().ToString();
            var newUser = new TenantUser
            {
                UserTenantId = id, UserObjectId = userId,
                UserName = Username, UserRole = Role
                
            };
            
            var query = new TenantService(_tenantDbContext);
            
            //Act
            _tenantDbContext.Tenant.Add(newTenant);
            _tenantDbContext.SaveChanges();
            _tenantDbContext.TenantUser.Add(newUser);
            _tenantDbContext.SaveChanges();

            var theme = query.GetTenantTheme(userId);

            var tenant = _tenantDbContext.Tenant.Find(id);
            var user = _tenantDbContext.TenantUser.Find(userId);

            //Assert
            Assert.NotNull(tenant);
            Assert.NotNull(user);
            Assert.NotNull(theme);
            Assert.Equal("Default", tenant.TenantTheme);
            
            //Delete from the database
            _tenantDbContext.TenantUser.Remove(user);
            _tenantDbContext.SaveChanges();
            _tenantDbContext.Tenant.Remove(tenant);
            _tenantDbContext.SaveChanges();
        } 
        
        //NEGATIVE TEST
        [Fact]
        public void Test_Setting_Theme_Fail_User_Null()
        {
            //Arrange
            var query = new TenantService(_tenantDbContext);
            
            //Act
            var exception = Assert.Throws<Exception>(() => query.GetTenantTheme(null));

            //Assert
            Assert.Equal("User id is null", exception.Message);
        } 

    }
}