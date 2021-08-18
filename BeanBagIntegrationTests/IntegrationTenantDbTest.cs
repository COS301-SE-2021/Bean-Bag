using BeanBag;
using BeanBag.Database;
using BeanBag.Models;
using BeanBag.Services;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
        public void Get_Tenants_From_Database_Success()
        {
            //Arrange
            var tenantId1 = Guid.NewGuid();
            var tenantName1 = "Tenant-1";
            var tenant1 = new Tenant {TenantId = tenantId1.ToString(), TenantName = tenantName1};
            
            var tenantId2 = Guid.NewGuid();
            var tenantName2 = "Tenant-2";
            var tenant2 = new Tenant {TenantId = tenantId2.ToString(), TenantName = tenantName2};
            
            var query = new TenantService(_tenantDbContext);
            var tenantList = query.GetTenantList();
            
            
            //Act
            var queryExisting = new TenantService(_tenantDbContext);
            var existing = queryExisting.GetTenantList().Count();
            
            _tenantDbContext.Tenant.Add(tenant1);
            _tenantDbContext.Tenant.Add(tenant2);
            _tenantDbContext.SaveChanges();
            
            var tenant = _tenantDbContext.Tenant.Find(tenantId1.ToString());

            
            //Assert
            Assert.Equal(2+existing,tenantList.Count());
            Assert.Equal(tenantId1.ToString(),tenant.TenantId);
            Assert.Equal("Tenant-1", tenant.TenantName);
            
            
            //Delete tenants from database
            _tenantDbContext.Remove(_tenantDbContext.Tenant.Find(tenantId1.ToString()));
            _tenantDbContext.Remove(_tenantDbContext.Tenant.Find(tenantId2.ToString()));
            _tenantDbContext.SaveChanges();
        }
        
    }
}