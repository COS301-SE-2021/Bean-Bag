using Microsoft.EntityFrameworkCore;
using BeanBag.Models;

namespace BeanBag.Database
{
    // This class is used to create, read, update and delete records from the database tables
    public class DBContext : DbContext
    {
        // This is the constructor for the DBContext class
        // It takes in the options in which the options are the connection string to the Bean Bag DB
        public DBContext(DbContextOptions<DBContext> options) : base (options)
        {

        }

        // This is the set of inventories found in the inventory table of the DB
        public DbSet<Inventory> Inventories { get; set; }
        public DbSet<Item> Items { get; set; }
        public DbSet<UserRoles> UserRoles { get; set; }
    }
}
