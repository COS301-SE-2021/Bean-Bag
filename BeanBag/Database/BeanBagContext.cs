using Microsoft.EntityFrameworkCore;

namespace BeanBag.Database
{
    public class BeanBagContext : DbContext
    {
        public virtual DbSet<Models.ItemModel> Items { get; set; }
        public BeanBagContext(DbContextOptions<BeanBagContext> options) : base(options){

            }
    }
}
