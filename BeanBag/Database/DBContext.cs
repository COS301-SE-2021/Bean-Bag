using System;
using Microsoft.EntityFrameworkCore;
using BeanBag.Models;
using BeanBag.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BeanBag.Database
{
    // This class is used to create, read, update and delete records from the database tables
    public class DBContext : DbContext
    {
        private readonly ITenantService _tenantService;
        private DBContext _dbContext;
        private static IConfiguration _configuration;
        private string _connection;
        private static DBContext _context;

        // This is the constructor for the DBContext class
        // It takes in the options in which the options are the connection string to the Bean Bag DB
        public DBContext(DbContextOptions<DBContext> options) : base (options)
        {

        }

        public DBContext(string name)
        {
            _configuration = new ConfigurationBuilder().SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                .AddJsonFile("appsettings.local.json").Build();

            _connection = _configuration.GetValue<String>("Database:DefaultConnectionPart1") + name +
                          _configuration.GetValue<String>("Database:DefaultConnectionPart2");
            
            //Set up service provider
            var serviceProvider = new ServiceCollection()
                .AddEntityFrameworkSqlServer()
                .BuildServiceProvider();

            //Set up the context options builder
            var builder = new DbContextOptionsBuilder<DBContext>();
            builder.UseSqlServer(_connection).UseInternalServiceProvider(serviceProvider);
            _context = new DBContext(builder.Options);
            
        }

        public DBContext GetContext()
        {
            return _context;
        }

        // This is the set of inventories found in the inventory table of the DB
        public DbSet<Inventory> Inventories { get; set; }
        public DbSet<Item> Items { get; set; }
        public DbSet<AIModel> AIModels { get; set; }
        public DbSet<AIModelVersions> AIModelIterations { get; set; }
    }
}
