using Microsoft.EntityFrameworkCore;
using BeanBag.Models;

namespace BeanBag.Database
{
    public class TenantDbContext : DbContext
    {
        // constructor
        public TenantDbContext(DbContextOptions<TenantDbContext> contextOptions) : base(contextOptions)
        {
            
        }
        
        // Entities
        // Current tenant information
        public DbSet<Tenant> Tenant { get; set; }
        
        // Current user 
        public DbSet<TenantUser> TenantUser { get; set; }
        
    }
}