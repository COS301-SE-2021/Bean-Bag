using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace BeanBag.AzureSqlDatabase
{
    public class AzureDatabaseCreation
    {
        private static string _databaseName;
        
        private static string _resource;
        private static string _server;

        private static DateTime _start;
        private static DateTime _end;

        private readonly IConfiguration _configuration;

        public AzureDatabaseCreation(string name)
        {
            _configuration = new ConfigurationBuilder().SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                .AddJsonFile("appsettings.local.json").Build();

            _resource = _configuration.GetValue<String>("AzureSqlDatabase:Resource");
            _server = _configuration.GetValue<String>("AzureSqlDatabase:Server");

            _databaseName = name;
            

        }
        
        public async Task Create()
        {
            //Set up SQL manager
            var sqlManager = new AzureDatabaseManager(_resource, _server);
            
            try
            {
                //Record start time
                _start = DateTime.UtcNow;
                
                //Create the database
                var created = await sqlManager.CreateDatabase(_databaseName);
                
                
                //Record end time to determine elapsed time
                _end = DateTime.UtcNow;

                var totalTime = _end.Subtract(_start);

                if (created)
                {
                    Console.WriteLine("Database created");
                }
                else
                {
                    Console.WriteLine("Database creation unsuccessful");
                }


            }
            catch (Exception e)
            {
                Console.WriteLine("Creation failed");
                
            }

            Console.ReadKey();
        }
    }
}