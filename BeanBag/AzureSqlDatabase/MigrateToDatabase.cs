using System;
using BeanBag.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;


namespace BeanBag.AzureSqlDatabase
{
    public class MigrateToDatabase
    {
        private static IConfiguration _configuration;
        private readonly string _connection;
        private static DBContext _context;

        public MigrateToDatabase(string name)
        {
            _configuration = new ConfigurationBuilder().SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                .AddJsonFile("appsettings.local.json").Build();

            _connection = _configuration.GetValue<String>("Database:DefaultConnectionPart1") + name +
                          _configuration.GetValue<String>("Database:DefaultConnectionPart2");

        }

        public  void ApplyMigration()
        {
           //Set up service provider
            var serviceProvider = new ServiceCollection()
                .AddEntityFrameworkSqlServer()
                .BuildServiceProvider();

            //Set up the context options builder
            var builder = new DbContextOptionsBuilder<DBContext>();
            builder.UseSqlServer(_connection).UseInternalServiceProvider(serviceProvider);
            _context = new DBContext(builder.Options);
            
            //Migrate to the new database
            _context.Database.Migrate();
        }
        
        
    }
}