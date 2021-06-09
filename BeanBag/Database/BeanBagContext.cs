using System;
using System.Collections.Generic;
using System.Linq;
using BeanBag.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace BeanBag.Database
{
    public class BeanBagContext : DbContext
    {
        protected BeanBagContext(){}
        public virtual DbSet<ItemModel> Items { get; set;}

        public BeanBagContext(DbContextOptions<BeanBagContext> options) : base(options)
        {

        }
    }
    
}