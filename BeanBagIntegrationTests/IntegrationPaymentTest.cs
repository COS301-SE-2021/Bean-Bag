using BeanBag;
using BeanBag.Database;
using BeanBag.Models;
using BeanBag.Services;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Xunit;

namespace BeanBagIntegrationTests
{
    public class IntegrationPaymentTest 
    {

        private readonly DBContext _db;
        private readonly TenantDbContext _Tdb;
        private readonly IConfiguration config;

        public IntegrationPaymentTest()
        {
            this.config = new ConfigurationBuilder().AddJsonFile("appsettings.local.json").Build();
            
            var serviceProvider = new ServiceCollection()
                .AddEntityFrameworkSqlServer()
                .BuildServiceProvider();

            var builder = new DbContextOptionsBuilder<TenantDbContext>();

            var connString = config.GetValue<string>("Database:TenantConnection");
            
            builder.UseSqlServer(connString).UseInternalServiceProvider(serviceProvider);

            _Tdb = new TenantDbContext(builder.Options);


        }

        [Fact]
        public void Add_Transaction_Test()
        {
            //ARRANGE
            var chars = "0123456789";
            var stringChars = new char[5];
            var random = new Random();

            for (var i = 0; i < stringChars.Length; i++)
            {
                stringChars[i] = chars[random.Next(chars.Length)];
            }

            var finalString = new String(stringChars);

            var myGuidEnd = finalString;

            var u2 = finalString.Substring(0, 4);
            
            Guid theId2 = new("00000000-0000-0000-0000-0000000" + myGuidEnd);
            
            Guid theIdPay = new("00000000-0000-0000-0000-0160000" + myGuidEnd);
            
            string tReference = "testtransaction";
            string tPayId = theIdPay.ToString();
            string tTenantId = theId2.ToString();
            float tAmount = 12.50f;



            //ACT
            var mySer = new PaymentService(_Tdb);
            var addedTransact = mySer.AddTransaction(tReference, tPayId, tTenantId, tAmount);

            //ASSERT
            Assert.True(addedTransact);
        }
        
        /*Dictionary<string, string> ToDictionary(string response);
        string GetMd5Hash(Dictionary<string, string> data, string encryptionKey);
        bool VerifyMd5Hash(Dictionary<string, string> data, string encryptionKey, string hash);
        public bool AddTransaction(string reference, string payId, string tenantId, float amount);
        IEnumerable<Transactions> GetTransactions(string currentTenantId);
        public Transactions GetPaidSubscription(string tenantId);
        public void UpdateSubscription(string subscription, string tenantId);
        
            */
    }
}
