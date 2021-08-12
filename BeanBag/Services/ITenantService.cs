﻿using System.Collections.Generic;
using BeanBag.Models;

namespace BeanBag.Services
{
    public interface ITenantService
    {
        public string GetTenantName(string tenantId);

        public string GetUserTenantId(string userId);

        public string GetTenantTheme(string userId);

        public bool SetTenantTheme(string userId, string theme);
        
        public bool CreateNewTenant(string tenantName);

        public void SetCurrentTenant(string tenant);
        
        public string GetCurrentTenant();

        public bool SearchTenant(string tenantId);

        public List<Tenant> GetTenantList();

        public bool SignUserUp(string userId, string tenantId);

        public bool SearchUser(string userId);

    }
}