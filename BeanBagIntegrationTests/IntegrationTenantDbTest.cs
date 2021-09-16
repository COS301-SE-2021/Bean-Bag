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
            const string name = "Tenant-name";
            const string address = "test-address";
            const string email = "tenant@test.com";
            const string number = "0123456789";
            const string subscription = "Free";

            var query = new TenantService(_tenantDbContext);
            
            //Act
            var created = query.CreateNewTenant(name,address,email,number,subscription);
            var tenantId = query.GetTenantId(name);
            
            var tenant = _tenantDbContext.Tenant.Find(tenantId);

            //Assert
            Assert.True(created);
            Assert.NotNull(tenantId);
            Assert.NotNull(tenant);
            Assert.Equal(name,tenant.TenantName);
            Assert.Equal(address,tenant.TenantAddress);
            Assert.Equal(email,tenant.TenantEmail);
            Assert.Equal(number,tenant.TenantNumber);
            Assert.Equal(subscription,tenant.TenantSubscription);
            
            //Delete from the database
            _tenantDbContext.Tenant.Remove(tenant);
            _tenantDbContext.SaveChanges();

        }
        
        //NEGATIVE TEST
        [Fact]
        public void Test_Tenant_Creation_Fail_Tenant_Name_Is_Null()
        {
            //Arrange
            const string address = "test-address";
            const string email = "tenant@test.com";
            const string number = "0123456789";
            const string subscription = "Free";
            
            var query = new TenantService(_tenantDbContext);

            //Act
            var exception = Assert.Throws<Exception>(() => query.CreateNewTenant(null,address,email,number,subscription));

            //Assert
            Assert.Equal("Tenant name is null", exception.Message);
        }
        
        
        //Test to get a list of tenants from the database
        //POSITIVE TEST
        [Fact]
        public void Test_Get_Tenants_From_Database_Success_Tenants_Added()
        {
            //Arrange
            var tenantId1 = Guid.NewGuid().ToString();
            var tenantName1 = "Tenant-1";
            var tenant1 = new Tenant {TenantId = tenantId1, TenantName = tenantName1};
            
            var tenantId2 = Guid.NewGuid().ToString();
            var tenantName2 = "Tenant-2";
            var tenant2 = new Tenant {TenantId = tenantId2, TenantName = tenantName2};

            //Act
            var queryExisting = new TenantService(_tenantDbContext);
            var existing = queryExisting.GetTenantList().Count();
            
            _tenantDbContext.Tenant.Add(tenant1);
            _tenantDbContext.SaveChanges();
            _tenantDbContext.Tenant.Add(tenant2);
            _tenantDbContext.SaveChanges();
            
            var query = new TenantService(_tenantDbContext);
            var tenantList = query.GetTenantList();
            
            var tenant = _tenantDbContext.Tenant.Find(tenantId1.ToString());
            
            //Assert
            Assert.NotNull(tenantList);
            Assert.Equal(2+existing,tenantList.Count());
            Assert.Equal(tenantId1,tenant.TenantId);
            Assert.Equal("Tenant-1", tenant.TenantName);

            //Delete tenants from database
            _tenantDbContext.Tenant.Remove(_tenantDbContext.Tenant.Find(tenantId1.ToString()));
            _tenantDbContext.SaveChanges();
            _tenantDbContext.Tenant.Remove(_tenantDbContext.Tenant.Find(tenantId2.ToString()));
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
        [Fact]
        public void Test_Get_Tenant_Id_Success_Tenant_Exists()
        {
            //Arrange
            var id = Guid.NewGuid();
            const string name = "Tenant-name";
            const string address = "test-address";
            const string email = "tenant@test.com";
            const string number = "0123456789";
            const string subscription = "Free";
            
            var newTenant = new Tenant
            {
                TenantId = id.ToString(), TenantName = name, TenantAddress = address, 
                TenantEmail = email, TenantNumber = number, TenantSubscription = subscription
            };

            //Act
            var query = new TenantService(_tenantDbContext);
            _tenantDbContext.Tenant.Add(newTenant);
            _tenantDbContext.SaveChanges();

            var tenantId = query.GetTenantId(name);

            var tenant = _tenantDbContext.Tenant.Find(newTenant.TenantId);

            //Assert
            Assert.NotNull(tenant);
            Assert.NotNull(tenantId);
            Assert.Equal(id.ToString(), tenantId);
            Assert.Equal(name, tenant.TenantName);

            //Delete from database
            _tenantDbContext.Remove(tenant);
            _tenantDbContext.SaveChanges();
        }
        
        //NEGATIVE TEST
        [Fact]
        public void Test_Get_Tenant_Id_Failure_Tenant_Does_Not_Exist()
        {
            //Arrange
            var id = Guid.NewGuid().ToString();
            var tenantName = "Tenant-name";
            
            var newTenant = new Tenant { TenantId = id, TenantName = tenantName };

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
            var name = "Tenant-name";
            var newTenant = new Tenant { TenantId = id, TenantName = name };
            
            //User
            var userId = Guid.NewGuid().ToString();
            var newUser = new TenantUser { UserTenantId = id, UserObjectId = userId };

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
            Assert.Equal("Tenant-name", tenantName);

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

           // var exceptionId = Assert.Throws<Exception>(() => query.GetTenantName());

            //Assert
            Assert.Null(tenant);
         //   Assert.Equal("User tenant id is null.", exceptionId.Message);

        }
        
        //Tenant search
        //POSITIVE TEST
        [Fact]
        public void Test_Tenant_Search_Success_Tenant_Found()
        {
            //Arrange
            //Tenant
            var id = Guid.NewGuid().ToString();
            var name = "Tenant-name";
            var newTenant = new Tenant { TenantId = id, TenantName = name };
            
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
            
           // var exception = Assert.Throws<Exception>(() => query.SearchTenant(null));
            
            //Assert
            Assert.Null(tenant);
            Assert.False(searchTenant);
         //   Assert.Equal("Tenant is null", exception.Message);
        }
        
        
        //Test getting tenant id from current user
        //POSITIVE TEST
        [Fact]
        public void Test_Tenant_Id_Retrieval_Success_Tenant_Found()
        {
            //Arrange
            //Tenant
            var id = Guid.NewGuid().ToString();
            var name = "Tenant-name";
            var newTenant = new Tenant { TenantId = id, TenantName = name };
            
            //User
            var userId = Guid.NewGuid().ToString();
            var newUser = new TenantUser { UserTenantId = id, UserObjectId = userId };

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
            var newTenant = new Tenant { TenantId = id, TenantName = "Tenant-name" };

            var userId = Guid.NewGuid().ToString();
            var newUser = new TenantUser {UserObjectId = userId, UserTenantId = id};
            
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
            var newTenant = new Tenant { TenantId = id, TenantName = "Tenant-name", TenantTheme = "Default"};

            var userId = Guid.NewGuid().ToString();
            var newUser = new TenantUser {UserObjectId = userId, UserTenantId = id};
            
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