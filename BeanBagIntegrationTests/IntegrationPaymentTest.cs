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
        

        
        [Fact]
        public void Add_Transaction_Test_Reference_Null()
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
            
            string tReference = null;
            string tPayId = theIdPay.ToString();
            string tTenantId = theId2.ToString();
            float tAmount = 12.50f;



            //ACT
            var mySer = new PaymentService(_Tdb);
            void Act() => mySer.AddTransaction(tReference, tPayId, tTenantId, tAmount);
            //var tr = mySer.AddTransaction(tReference, tPayId, tTenantId, tAmount);

            //ASSERT
            var exception = Assert.Throws<Exception>(Act);
            Assert.Equal("Reference data is null", exception.Message);
            //Assert.False(tr);
            
        }
        
        [Fact]
        public void Add_Transaction_Test_PayId_Null()
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
            string tPayId = null;
            string tTenantId = theId2.ToString();
            float tAmount = 12.50f;



            //ACT
            var mySer = new PaymentService(_Tdb);
            void Act() => mySer.AddTransaction(tReference, tPayId, tTenantId, tAmount);
            

            //ASSERT
            var exception = Assert.Throws<Exception>(Act);
            Assert.Equal("PayId is null.", exception.Message);
            
        }
        
        [Fact]
        public void Add_Transaction_Test_TenantId_Null()
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
            string tTenantId = null;
            float tAmount = 12.50f;



            //ACT
            var mySer = new PaymentService(_Tdb);
            void Act() => mySer.AddTransaction(tReference, tPayId, tTenantId, tAmount);
            

            //ASSERT
            var exception = Assert.Throws<Exception>(Act);
            Assert.Equal("TenantId is null.", exception.Message);
            
        }
        
        [Fact]
        public void Add_Transaction_Test_Amount_Null()
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
            float tAmount = 0;
            
            //ACT
            var mySer = new PaymentService(_Tdb);
            void Act() => mySer.AddTransaction(tReference, tPayId, tTenantId, tAmount);

            //ASSERT
            var exception = Assert.Throws<Exception>(Act);
            Assert.Equal("Amount is null.", exception.Message);
            
        }
        
        [Fact]
        public void Get_Transactions_valid()
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

            string tReference = "testtransactionwNada";
            string tPayId = theIdPay.ToString();
            string tTenantId = theId2.ToString();
            float tAmount = 12.50f;

            string currentTenantIdTest = tTenantId;

            //ACT
            var mySer = new PaymentService(_Tdb);
            var isCheck = mySer.AddTransaction(tReference, tPayId, tTenantId, tAmount);
            var myTrns = mySer.GetTransactions(currentTenantIdTest);
            var icount = myTrns.Count();
            var myl = myTrns.ToList();

            //ASSERT
            Assert.Equal(1, icount);
            mySer.DeleteTransaction(myl[0].TransactionId);
        }
        
        /*Dictionary<string, string> ToDictionary(string response);
        string GetMd5Hash(Dictionary<string, string> data, string encryptionKey);
        bool VerifyMd5Hash(Dictionary<string, string> data, string encryptionKey, string hash);
        public bool AddTransaction(string reference, string payId, string tenantId, float amount);
        
        public Transactions GetPaidSubscription(string tenantId);
        public void UpdateSubscription(string subscription, string tenantId);
        
            */
    }
}
