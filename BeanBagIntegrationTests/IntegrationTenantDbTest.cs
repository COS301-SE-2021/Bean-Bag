using BeanBag.Database;
using BeanBag.Models;
using BeanBag.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using Xunit;

namespace BeanBagIntegrationTests
{
    public class IntegrationTenantDbTest
    {
        private readonly TenantDbContext _tenantDbContext;

        public IntegrationTenantDbTest()
        {
            var serviceProvider = new ServiceCollection()
                .AddEntityFrameworkSqlServer()
                .BuildServiceProvider();

            var builder = new DbContextOptionsBuilder<TenantDbContext>();
            builder.UseSqlServer("Server=tcp:polariscapestone.database.windows.net,1433;Initial Catalog=Bean-Bag-Tenants;Persist Security Info=False;User ID=polaris;Password=MNRSSp103;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;")
                .UseInternalServiceProvider(serviceProvider);

            _tenantDbContext = new TenantDbContext(builder.Options);
        }
        
        
        //Test to get a list of tenants from the database
        //POSITIVE TEST
        [Fact]
        public void Get_Tenants_From_Database_Success_Tenants_Added()
        {
            //Arrange
            var tenantId1 = Guid.NewGuid();
            var tenantName1 = "Tenant-1";
            var tenant1 = new Tenant {TenantId = tenantId1.ToString(), TenantName = tenantName1};
            
            var tenantId2 = Guid.NewGuid();
            var tenantName2 = "Tenant-2";
            var tenant2 = new Tenant {TenantId = tenantId2.ToString(), TenantName = tenantName2};

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
            Assert.Equal(tenantId1.ToString(),tenant.TenantId);
            Assert.Equal("Tenant-1", tenant.TenantName);

            //Delete tenants from database
            _tenantDbContext.Tenant.Remove(_tenantDbContext.Tenant.Find(tenantId1.ToString()));
            _tenantDbContext.SaveChanges();
            _tenantDbContext.Tenant.Remove(_tenantDbContext.Tenant.Find(tenantId2.ToString()));
            _tenantDbContext.SaveChanges();
        }
        
        //NEGATIVE TEST
        [Fact]
        public void Get_Tenants_From_Database_Fail_Tenants_Not_Added()
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
        public void Get_Tenant_Id_Success_Tenant_Exists()
        {
            //Arrange
            var id = Guid.NewGuid();
            var tenantName = "Tenant-name";
            
            var newTenant = new Tenant { TenantId = id.ToString(), TenantName = tenantName };

            //Act
            var query = new TenantService(_tenantDbContext);
            _tenantDbContext.Tenant.Add(newTenant);
            _tenantDbContext.SaveChanges();

            var tenantId = query.GetTenantId(tenantName);

            var tenant = _tenantDbContext.Tenant.Find(newTenant.TenantId);

            //Assert
            Assert.NotNull(tenant);
            Assert.NotNull(tenantId);
            Assert.Equal(id.ToString(), tenantId);
            Assert.Equal("Tenant-name", tenant.TenantName);

            //Delete from database
            _tenantDbContext.Remove(tenant);
            _tenantDbContext.SaveChanges();
        }
        
        //NEGATIVE TEST
        [Fact]
        public void Get_Tenant_Id_Failure_Tenant_Does_Not_Exist()
        {
            //Arrange
            var id = Guid.NewGuid();
            var tenantName = "Tenant-name";
            
            var newTenant = new Tenant { TenantId = id.ToString(), TenantName = tenantName };

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
        public void Get_Tenant_Name_Success_Tenant_Exists()
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
        public void Get_Tenant_Name_Fail_Tenant_Is_Null()
        {
            //Arrange
            //Tenant
            var id = Guid.NewGuid().ToString();
            const string name = "Tenant-name";

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
            var name = "Tenant-name";
            var newTenant = new Tenant { TenantId = id, TenantName = name };
            
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
        
        
        //User search
        //POSITIVE TEST
        [Fact]
        public void Test_User_Search_Success_User_Found()
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

            var searchUser = query.SearchUser(userId);

            //Assert
            Assert.NotNull(user);
            Assert.NotNull(tenant);
            Assert.True(searchUser);
            
            //Delete from database
            _tenantDbContext.TenantUser.Remove(user);
            _tenantDbContext.SaveChanges();
            _tenantDbContext.Tenant.Remove(tenant);
            _tenantDbContext.SaveChanges();
        }
        
        //NEGATIVE TEST
        [Fact]
        public void Test_User_Search_Fail_User_Not_Found()
        {
            //Arrange
            //Tenant
            var id = Guid.NewGuid().ToString();

            //User
            var userId = Guid.NewGuid().ToString();

            //Act
            var query = new TenantService(_tenantDbContext);

            var tenant = _tenantDbContext.Tenant.Find(id);
            var user = _tenantDbContext.TenantUser.Find(userId);

            var searchUser = query.SearchUser(userId);
            var exception = Assert.Throws<Exception>(() => query.SearchUser(null));

            //Assert
            Assert.Null(user);
            Assert.Null(tenant);
            Assert.False(searchUser);
            Assert.Equal("User is null", exception.Message);
        }
        
        
        
        //User sign up
        //POSITIVE TEST
        [Fact]
        public void Test_User_Sign_Up_Success_User_Added()
        {
            //Arrange
            
            //Tenant
            var id = Guid.NewGuid().ToString();
            var newTenant = new Tenant { TenantId = id, TenantName = "Tenant-name" };
            
            //User
            var userId = Guid.NewGuid().ToString();
            
            
            //Act
            var query = new TenantService(_tenantDbContext);
            _tenantDbContext.Tenant.Add(newTenant);
            _tenantDbContext.SaveChanges();

            var signedUp = query.SignUserUp(userId, id);

            var tenant = _tenantDbContext.Tenant.Find(id);
            var user = _tenantDbContext.TenantUser.Find(userId);
            
            
            //Assert
            Assert.NotNull(tenant);
            Assert.NotNull(user);
            Assert.True(signedUp);
            Assert.Equal(userId, user.UserObjectId);
            Assert.Equal(id, user.UserTenantId);

            
            //Delete from database
            _tenantDbContext.TenantUser.Remove(user);
            _tenantDbContext.SaveChanges();
            _tenantDbContext.Tenant.Remove(tenant);
            _tenantDbContext.SaveChanges();
        }
        
        //NEGATIVE TEST
        [Fact]
        public void Test_User_Sign_Up_Fail_User_Not_Added_Already_Exists()
        {
            //Arrange
            
            //Tenant
            var id = Guid.NewGuid().ToString();
            var newTenant = new Tenant { TenantId = id, TenantName = "Tenant-name" };
            
            //User
            var userId = Guid.NewGuid().ToString();
            
            
            //Act
            var query = new TenantService(_tenantDbContext);
            _tenantDbContext.Tenant.Add(newTenant);
            _tenantDbContext.SaveChanges();

            var signedUp = query.SignUserUp(userId, id);
            var signedUpAgain = query.SignUserUp(userId, id);

            var tenant = _tenantDbContext.Tenant.Find(id);
            var user = _tenantDbContext.TenantUser.Find(userId);
            
            //Assert
            Assert.NotNull(tenant);
            Assert.NotNull(user);
            Assert.True(signedUp);
            Assert.False(signedUpAgain);
            
            //Delete from database
            _tenantDbContext.TenantUser.Remove(user);
            _tenantDbContext.SaveChanges();
            _tenantDbContext.Tenant.Remove(tenant);
            _tenantDbContext.SaveChanges();
        }
        
        //NEGATIVE TEST
        [Fact]
        public void Test_User_Sign_Up_Fail_User_Not_Added_Tenant_Does_Not_Exist()
        {
            //Arrange
            
            //Tenant
            var id = Guid.NewGuid().ToString();
            //User
            var userId = Guid.NewGuid().ToString();
            
            
            //Act
            var query = new TenantService(_tenantDbContext);

            var signedUp = query.SignUserUp(userId, id);
            
            var tenant = _tenantDbContext.Tenant.Find(id);
            var user = _tenantDbContext.TenantUser.Find(userId);
            
            //Assert
            Assert.Null(tenant);
            Assert.Null(user);
            Assert.False(signedUp);

        }
        
    }
}