using BeanBag.Models;
using Microsoft.EntityFrameworkCore;

namespace BeanBag.Database
{
    // This class is used to create a database context.
    public class BeanBagContext : DbContext
    {
        protected BeanBagContext(){}
        public virtual DbSet<ItemModel> Items { get; set;}

        public BeanBagContext(DbContextOptions<BeanBagContext> options) : base(options)
        {

        }
    }
    
}