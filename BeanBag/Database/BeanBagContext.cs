using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace BeanBag.Database
{
    public class BeanBagContext : DbContext
    {
        public virtual DbSet<BeanBag.Models.ItemModel> Items { get; set; }
        public BeanBagContext(DbContextOptions<BeanBagContext> options) : base(options){

            }
    }
}
