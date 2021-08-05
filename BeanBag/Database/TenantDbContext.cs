using Microsoft.EntityFrameworkCore;
using BeanBag.Models;

namespace BeanBag.Database
{
    public class TenantDbContext : DbContext
    {
        //Constructor
        public TenantDbContext(DbContextOptions<TenantDbContext> contextOptions) : base(contextOptions)
        {
            
        }
        
        public DbSet<Tenant> Tenant { get; set; }
        public DbSet<TenantUser> TenantUser { get; set; }

    }
}