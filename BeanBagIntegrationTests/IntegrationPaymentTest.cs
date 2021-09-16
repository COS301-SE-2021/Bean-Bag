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
            var myTrns = mySer.GetTransactions(tTenantId);
            var icount = myTrns.Count();
            var myl = myTrns.ToList();

            //ASSERT
            Assert.True(addedTransact);
            mySer.DeleteTransaction(myl[0].TransactionId);
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
        
        [Fact]
        public void Get_Transactions_Invalid_id()
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
            Guid theIdx = new("10000000-0000-0000-0000-0000000" + myGuidEnd);

            Guid theIdPay = new("00000000-0000-0000-0000-0160000" + myGuidEnd);

            string tReference = "testtransactionwNada";
            string tPayId = theIdPay.ToString();
            string tTenantId = theId2.ToString();
            float tAmount = 12.50f;

            string currentTenantIdTest = null;

            //ACT
            var mySer = new PaymentService(_Tdb);
            var isCheck = mySer.AddTransaction(tReference, tPayId, tTenantId, tAmount);
            void Act() => mySer.GetTransactions(currentTenantIdTest);
            var myTrns = mySer.GetTransactions(tTenantId);

            var icount = myTrns.Count();
            var myl = myTrns.ToList();
            

            //ASSERT
            var exception = Assert.Throws<Exception>(Act);
            Assert.Equal("Tenant id is null", exception.Message);
            mySer.DeleteTransaction(myl[0].TransactionId);

        }
        
        
        [Fact]
        public void Get_Paid_Subscription_valid()
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
            
            var myrtransaction = mySer.GetPaidSubscription(currentTenantIdTest);
            
            var icount = myTrns.Count();
            var myl = myTrns.ToList();

            //ASSERT
            Assert.Equal(1, icount);
            Assert.NotNull(myrtransaction);
            mySer.DeleteTransaction(myl[0].TransactionId);
        }
        
        
        [Fact]
        public void Get_Paid_Subscription_Invalid()
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

            string currentTenantIdTest = null;

            //ACT
            var mySer = new PaymentService(_Tdb);
            var isCheck = mySer.AddTransaction(tReference, tPayId, tTenantId, tAmount);
            var myTrns = mySer.GetTransactions(tTenantId);
            
            void Act() => mySer.GetPaidSubscription(currentTenantIdTest);
            
            var icount = myTrns.Count();
            var myl = myTrns.ToList();

            //ASSERT
            var exception = Assert.Throws<Exception>(Act);
            Assert.Equal("TenantID is null", exception.Message);
            mySer.DeleteTransaction(myl[0].TransactionId);
        }
        
        [Fact]
        public void Delete_Transaction_Invalid()
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
            //var addedTransact = mySer.AddTransaction(tReference, tPayId, tTenantId, tAmount);
            var myTrns = mySer.GetTransactions(tTenantId);
            var icount = myTrns.Count();
            var myl = myTrns.ToList();
            var isDel = mySer.DeleteTransaction(theId2.ToString());
            
            //ASSERT
            Assert.False(isDel);
            
        }
        
        [Fact]
        public void Update_Subscription_Valid_Free()
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
            
            Guid theIdPay = new("00100000-0000-0000-0000-0160000" + myGuidEnd);
            
            string tReference = "testtransaction";
            string tPayId = theIdPay.ToString();
            string tTenantId = theId2.ToString();
            float tAmount = 12.50f;



            //ACT
            var mySer = new PaymentService(_Tdb);
            var addedTransact = mySer.AddTransaction(tReference, tPayId, tTenantId, tAmount);
            var myTrns = mySer.GetTransactions(tTenantId);
            
            mySer.UpdateSubscription("Free", tTenantId);
            
            var icount = myTrns.Count();
            var myl = myTrns.ToList();
            
            
            //ASSERT
            Assert.True(addedTransact);
            var isDel = mySer.DeleteTransaction(myl[0].TransactionId);
        }
        
        [Fact]
        public void Update_Subscription_Valid_Standard()
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
            var myTrns = mySer.GetTransactions(tTenantId);
            
            mySer.UpdateSubscription("Standard", tTenantId);
            
            var icount = myTrns.Count();
            var myl = myTrns.ToList();
            
            
            //ASSERT
            Assert.True(addedTransact);
            var isDel = mySer.DeleteTransaction(myl[0].TransactionId);
        }
        
        [Fact]
        public void Update_Subscription_Valid_Premium()
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
            var myTrns = mySer.GetTransactions(tTenantId);
            
            mySer.UpdateSubscription("Premium", tTenantId);
            
            var icount = myTrns.Count();
            var myl = myTrns.ToList();
            
            
            //ASSERT
            Assert.True(addedTransact);
            var isDel = mySer.DeleteTransaction(myl[0].TransactionId);
        }
        
        [Fact]
        public void Update_Subscription_Invalid_Sub()
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
            var myTrns = mySer.GetTransactions(tTenantId);
            
            void Act() => mySer.UpdateSubscription("NotAChoice", tTenantId);
            
            var icount = myTrns.Count();
            var myl = myTrns.ToList();
            
            
            //ASSERT
            var exception = Assert.Throws<Exception>(Act);
            Assert.Equal("Incorrect subscription input.", exception.Message);
            Assert.True(addedTransact);
            var isDel = mySer.DeleteTransaction(myl[0].TransactionId);
        }
        
        [Fact]
        public void Update_Subscription_null_Sub()
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
            var myTrns = mySer.GetTransactions(tTenantId);
            
            void Act() => mySer.UpdateSubscription(null, tTenantId);
            
            var icount = myTrns.Count();
            var myl = myTrns.ToList();
            
            
            //ASSERT
            var exception = Assert.Throws<Exception>(Act);
            Assert.Equal("Subscription is null", exception.Message);
            Assert.True(addedTransact);
            var isDel = mySer.DeleteTransaction(myl[0].TransactionId);
        }


        [Fact]
        public void Update_tenant_null()
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
            var myTrns = mySer.GetTransactions(tTenantId);
            
            void Act() => mySer.UpdateSubscription("Free", null);
            
            var icount = myTrns.Count();
            var myl = myTrns.ToList();
            
            
            //ASSERT
            var exception = Assert.Throws<Exception>(Act);
            Assert.Equal("Tenant id is null", exception.Message);
            Assert.True(addedTransact);
            var isDel = mySer.DeleteTransaction(myl[0].TransactionId);
        }
        
        
        //HASHING ALGORITHM TESTS

        [Fact]
        public void To_Url_Encoded_string_valid()
        {
            //ARRANGE
            var _request = new Dictionary<string, string>();
            var mySer = new PaymentService(_Tdb);

            //ACT
            var ires = mySer.ToUrlEncodedString(_request);

            //ASSERT
            Assert.Equal("",ires);
        }
        
        [Fact]
        public void To_Url_Encoded_string_null_input()
        {
            //ARRANGE
            var _request = new Dictionary<string, string>();
            _request = null;
            var mySer = new PaymentService(_Tdb);

            //ACT
            void Act() => mySer.ToUrlEncodedString(_request);

            //ASSERT
            var exception = Assert.Throws<Exception>(Act);
            Assert.Equal("Input string is null", exception.Message);
            
        }
        
        [Fact]
        public void To_Dictionary_valid()
        {
            //ARRANGE
            string _request = "https://example.com/path/to/page?name=ferret&color=purple";
            
            var mySer = new PaymentService(_Tdb);

            //ACT
            var ires = mySer.ToDictionary(_request);

            //ASSERT
            Assert.IsType<Dictionary<string, string>>(ires);
            
        }
        
        [Fact]
        public void To_Dictionary_input_null()
        {
            //ARRANGE
            string _request = null;
            
            var mySer = new PaymentService(_Tdb);

            //ACT
            void Act() => mySer.ToDictionary(_request);

            //ASSERT
            var exception = Assert.Throws<Exception>(Act);
            Assert.Equal("Input string is null", exception.Message);
            
        }
        
        [Fact]
        public void Get_Md5_hash_valid()
        {
            //ARRANGE
            var _request = new Dictionary<string, string>();
            string _enryptionKey = "10101";
            var mySer = new PaymentService(_Tdb);

            //ACT
            var ires = mySer.GetMd5Hash(_request, _enryptionKey);

            //ASSERT
            Assert.Equal("6dfc35c47756e962ef055d1049f1f8ec",ires);
        }
        
        [Fact]
        public void Get_Md5_hash_dictionary_null()
        {
            //ARRANGE
            var _request = new Dictionary<string, string>();
            _request = null;
            string _enryptionKey = "10101";
            var mySer = new PaymentService(_Tdb);

            //ACT
            void Act() => mySer.GetMd5Hash(_request, _enryptionKey);

            //ASSERT
            var exception = Assert.Throws<Exception>(Act);
            Assert.Equal("Dictionary data is null", exception.Message);
        }
        
        [Fact]
        public void Get_Md5_hash_key_null()
        {
            //ARRANGE
            var _request = new Dictionary<string, string>();
            string _enryptionKey = null;
            var mySer = new PaymentService(_Tdb);

            //ACT
            void Act() => mySer.GetMd5Hash(_request, _enryptionKey);

            //ASSERT
            var exception = Assert.Throws<Exception>(Act);
            Assert.Equal("EncryptionKey is null.", exception.Message);
        }
        
        [Fact]
        public void Verify_Md5_hash_data_null()
        {
            //ARRANGE
            var _request = new Dictionary<string, string>();
            _request = null;
            string _enryptionKey = "10101";
            string _hash = "6dfc35c47756e962ef055d1049f1f8ec";
            var mySer = new PaymentService(_Tdb);

            //ACT
            void Act() => mySer.VerifyMd5Hash(_request, _enryptionKey, _hash);

            //ASSERT
            var exception = Assert.Throws<Exception>(Act);
            Assert.Equal("Dictionary data is null", exception.Message);
        }
        
        [Fact]
        public void Verify_Md5_hash_key_null()
        {
            //ARRANGE
            var _request = new Dictionary<string, string>();
            string _enryptionKey = null;
            string _hash = "6dfc35c47756e962ef055d1049f1f8ec";
            var mySer = new PaymentService(_Tdb);

            //ACT
            void Act() => mySer.VerifyMd5Hash(_request, _enryptionKey, _hash);

            //ASSERT
            var exception = Assert.Throws<Exception>(Act);
            Assert.Equal("EncryptionKey is null.", exception.Message);
        }
        
        [Fact]
        public void Verify_Md5_hash_hash_null()
        {
            //ARRANGE
            var _request = new Dictionary<string, string>();
            string _enryptionKey = "10101";
            string _hash = null;
            var mySer = new PaymentService(_Tdb);

            //ACT
            void Act() => mySer.VerifyMd5Hash(_request, _enryptionKey, _hash);

            //ASSERT
            var exception = Assert.Throws<Exception>(Act);
            Assert.Equal("Hash is null.", exception.Message);
        }
        
        [Fact]
        public void Verify_Md5_hash_valid()
        {
            //ARRANGE
            var _request = new Dictionary<string, string>();
            string _enryptionKey = "10101";
            string _hash = "6dfc35c47756e962ef055d1049f1f8ec";
            var mySer = new PaymentService(_Tdb);

            //ACT
            var isDone = mySer.VerifyMd5Hash(_request, _enryptionKey, _hash);

            //ASSERT
            Assert.True(isDone);
        }
        

        
        
        /*
        Dictionary<string, string> ToDictionary(string response);
        string GetMd5Hash(Dictionary<string, string> data, string encryptionKey);
        bool VerifyMd5Hash(Dictionary<string, string> data, string encryptionKey, string hash);
        
        */
    }
}
