using BeanBag.Database;
using BeanBag.Models;
using BeanBag.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using Microsoft.Extensions.Configuration;
using Xunit;

namespace BeanBagIntegrationTests
{
    public class IntegrationTenantDbTestUser
    {
        private readonly TenantDbContext _tenantDbContext;
        
        private readonly IConfiguration _configuration;

        public IntegrationTenantDbTestUser()
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
    /*    [Fact]
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
        */
        //NEGATIVE TEST
 /*       [Fact]
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
        
        //NEGATIVE TEST
        [Fact]
        public void Test_User_Sign_Up_Fail_User_Not_Added_Id_Is_Null()
        {
            //Arrange
            
            //Tenant
            var id = Guid.NewGuid().ToString();
            //User
            var userId = Guid.NewGuid().ToString();
            
            
            //Act
            var query = new TenantService(_tenantDbContext);

            var exceptionTenant = Assert.Throws<Exception>(() => query.SignUserUp(userId, null));
            var exceptionUser = Assert.Throws<Exception>(() => query.SignUserUp(null, null));
            
            var tenant = _tenantDbContext.Tenant.Find(id);
            var user = _tenantDbContext.TenantUser.Find(userId);
            
            //Assert
            Assert.Null(tenant);
            Assert.Null(user);
            Assert.Equal("User or tenant id is null", exceptionTenant.Message);
            Assert.Equal("User or tenant id is null", exceptionUser.Message);

        }*/
    }
}