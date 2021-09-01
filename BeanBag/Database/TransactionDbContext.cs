using Microsoft.EntityFrameworkCore;
using BeanBag.Models;

namespace BeanBag.Database
{
     public class TransactionDbContext : DbContext
        {
            public TransactionDbContext(DbContextOptions<TransactionDbContext> contextOptions) : base(contextOptions) {}
            public DbSet<Transaction> Transaction { get; set; }

        }
}