using System;
using System.Collections.Generic;
using System.Linq;
using BeanBag.Database;
using BeanBag.Models;
using BeanBag.Services;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;

namespace BeanBagUnitTests
{
    


    public class UnitItemTests
    {
        public UnitItemTests(DbContextOptions<DBContext> contextOptions)
        {
            ContextOptions = contextOptions;

            Seed();
        }
        
        protected DbContextOptions<DBContext> ContextOptions { get; }

        private void Seed()
        {
            using (var context = new DBContext(ContextOptions))
            {
                context.Database.EnsureDeleted();
                context.Database.EnsureCreated();

                Guid theId1 = new("00000000-0000-0000-0000-000000000001");
                Guid theId2 = new("00000000-0000-0000-0000-000000000002");

                string u1 = "xxx";
                string u2 = "yyy";
                
                var one = new Inventory{Id = theId1, name = "Mum's 1st", userId = u1};
                var two = new Inventory{Id = theId2, name = "Mum's 2nd", userId = u1};

                context.AddRange(one, two);

                context.SaveChanges();
            }
        }

    }

    


}


