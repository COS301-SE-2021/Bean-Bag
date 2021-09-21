using Microsoft.EntityFrameworkCore;
using BeanBag.Models;

namespace BeanBag.Database
{
    //This class is the tenant Db Context
    public class TenantDbContext : DbContext
    {
        public TenantDbContext(DbContextOptions<TenantDbContext> contextOptions) : base(contextOptions) {}
        public DbSet<Tenant> Tenant { get; set; }
        public DbSet<TenantUser> TenantUser { get; set; }

        public DbSet<Transactions> Transactions { get; set; }
    }
}