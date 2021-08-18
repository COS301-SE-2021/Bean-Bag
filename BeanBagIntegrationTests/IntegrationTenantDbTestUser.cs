﻿using BeanBag.Database;
using BeanBag.Models;
using BeanBag.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using Xunit;

namespace BeanBagIntegrationTests
{
    public class IntegrationTenantDbTestUser
    {
        private readonly TenantDbContext _tenantDbContext;

        public IntegrationTenantDbTestUser()
        {
            var serviceProvider = new ServiceCollection()
                .AddEntityFrameworkSqlServer()
                .BuildServiceProvider();

            var builder = new DbContextOptionsBuilder<TenantDbContext>();
            builder.UseSqlServer("Server=tcp:polariscapestone.database.windows.net,1433;Initial Catalog=Bean-Bag-Tenants;Persist Security Info=False;User ID=polaris;Password=MNRSSp103;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;")
                .UseInternalServiceProvider(serviceProvider);

            _tenantDbContext = new TenantDbContext(builder.Options);
        }
    }
}