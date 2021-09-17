using System;
using System.Threading.Tasks;
using Microsoft.Azure.Management.ResourceManager.Fluent;
using Microsoft.Azure.Management.ResourceManager.Fluent.Authentication;
using Microsoft.Azure.Management.ResourceManager.Fluent.Core;
using Microsoft.Azure.Management.Sql.Fluent;
using Microsoft.Azure.Management.Sql.Fluent.Models;
using Microsoft.Extensions.Configuration;

namespace BeanBag.AzureSqlDatabase
{
    public class AzureDatabaseManager
    {

        private readonly string _azureResource;
        private readonly string _databaseServer;
        private readonly IConfiguration _configuration;

        private readonly SqlManagementClient _sqlClient;
        
        /* Constructor initialises the Azure resource and server to be used for the
         database creation */
        public AzureDatabaseManager(string resource, string server)
        {
            _azureResource = resource;
            _databaseServer = server;
            _configuration = new ConfigurationBuilder().SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                .AddJsonFile("appsettings.local.json").Build();

            //Azure credentials
            var applicationId = _configuration.GetValue<String>("AzureAd:ClientId");
            var tenantId = _configuration.GetValue<String>("AzureAd:TenantId");
            var subscriptionId = _configuration.GetValue<String>("AzureAd:SubscriptionId");
            var secret = _configuration.GetValue<String>("AzureAd:Secret");

            //Credential access
            var azureCredentials = new AzureCredentialsFactory()
                .FromServicePrincipal(applicationId, secret, tenantId, AzureEnvironment.AzureGlobalCloud)
                .WithDefaultSubscription(subscriptionId);
            
            
            //Configure the REST client
            var apiRestClient = RestClient.Configure()
                .WithEnvironment(AzureEnvironment.AzureGlobalCloud)
                .WithLogLevel(HttpLoggingDelegatingHandler.Level.Basic)
                .WithCredentials(azureCredentials)
                .Build();

            //Sql management client
            _sqlClient = new SqlManagementClient(apiRestClient);
            _sqlClient.SubscriptionId = subscriptionId;

        }

        //Async function to create the sql database in Azure
        public async Task<bool> CreateDatabase(string name)
        {
            var databaseDetails = new DatabaseInner
            {
                Location = "South Africa North",
                Collation = "SQL_Latin1_General_CP1_CI_AS",
                Edition = DatabaseEdition.Basic,
                CreateMode = CreateMode.Default
            };

            var creation =
                await _sqlClient.Databases.CreateOrUpdateAsync(_azureResource, _databaseServer, name, databaseDetails);
            
            

            //Test if database was created
            return !string.IsNullOrEmpty(creation.Name);
        }
    }
}