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
        public DbSet<Tenants> Tenant { get; set; }
        
        // Current user 
        public DbSet<Users> User { get; set; }
        
    }
}