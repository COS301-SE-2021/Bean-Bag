using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using BeanBag.Database;
using BeanBag.Models;

namespace BeanBag.Services
{
    // This service class is used to handle any data that is related to the payment and transaction process.
    // This service class main focus is to bridge the the payment controller to payment service functions.
    public class PaymentService : IPaymentService
    {
         private readonly TenantDbContext _tenantDb;

         // Constructor sets database context
         public PaymentService(TenantDbContext tenantDb)
         {
             _tenantDb = tenantDb;
         }
         
         #region Utilities
        
        // This function is used to Encode dictionary to Url string
        public string ToUrlEncodedString(Dictionary<string, string> request)
        {
            if (request == null)
            {
                throw new Exception("Input string is null");
            }
            string result;
            
            StringBuilder builder = new StringBuilder();
            foreach (string key in request.Keys)
            {
                builder.Append("&");
                builder.Append(key);
                builder.Append("=");
                builder.Append(HttpUtility.UrlEncode(request[key]));
            }

             result = builder.ToString().TrimStart('&');
             
            return result;
        }

        // This function converts a query string to dictionary 
        public Dictionary<string, string> ToDictionary(string response)
        {
            if (response == null)
            {
                throw new Exception("Input string is null");
            }
            
            Dictionary<string, string> result = new Dictionary<string, string>();
            try
            {
                string[] valuePairs = response.Split('&');
                foreach (string valuePair in valuePairs)
                {
                    string[] values = valuePair.Split('=');

                    result.Add(values[0], HttpUtility.UrlDecode(values[1]));
                }
            }
            catch(Exception)
            {
                throw new Exception("String could not be converted to dictionary.");
            }

            return result;
        }
        
        #endregion Utility
        
        // SEE SHA HASHING COULD BE MORE SECURE (Also 3D verification if there is time ) 
        
        #region MD5 Hashing
        // Adapted from
        // https://msdn.microsoft.com/en-us/library/system.security.cryptography.md5(v=vs.110).aspx

        // This function computes a hashcode, this is for security purposes
        public string GetMd5Hash(Dictionary<string, string> data, string encryptionKey)
        {
            if (data == null)
            {
                throw new Exception("Dictionary data is null");
            }
            else if (encryptionKey == null) 
            {
                throw new Exception("EncryptionKey is null.");
            }

            try
            {
                using MD5 md5Hash = MD5.Create();
                StringBuilder input = new StringBuilder();
                foreach (string value in data.Values)
                {
                    input.Append(value);
                }

                input.Append(encryptionKey);

                byte[] hash = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(input.ToString()));

                StringBuilder sBuilder = new StringBuilder();

                foreach (var t in hash)
                {
                    sBuilder.Append(t.ToString("x2"));
                }

                return sBuilder.ToString();
            }
            catch(Exception)
            {
                throw new Exception("Could not get Md5 hash.");
            }
        }

        // This function verifies that the encryption hashing took place. 
        public bool VerifyMd5Hash(Dictionary<string, string> data, string encryptionKey, string hash)
        {
            if (data == null)
            {
                throw new Exception("Dictionary data is null");
            }
            else if (encryptionKey == null) 
            {
                throw new Exception("EncryptionKey is null.");
            }
            else if (hash == null)
            {
                throw new Exception("Hash is null.");
            }
            
            Dictionary<string, string> hashDict = new Dictionary<string, string>();

            foreach (string key in data.Keys)
            {
                if (key != "CHECKSUM")
                {
                    hashDict.Add(key, data[key]);
                }
            }

            string hashOfInput = GetMd5Hash(hashDict, encryptionKey);

            StringComparer comparer = StringComparer.OrdinalIgnoreCase;

            if (0 == comparer.Compare(hashOfInput, hash))
            {
                return true;
            }
            else
            {
                Console.WriteLine("Checking hashing.....");
                Console.WriteLine(comparer.Compare(hashOfInput, hash));
                return false;
            }
        }

        #endregion MD5 Hash

        // This function adds a transaction to the database.
        public bool AddTransaction(string reference, string payId, string tenantId, float amount)
        {
            if (reference == null)
            {
                throw new Exception("Reference data is null");
            }
            else if (payId == null) 
            {
                throw new Exception("PayId is null.");
            }
            else if (tenantId == null)
            {
                throw new Exception("TenantId is null.");
            }else if (amount == 0)
            {
                throw new Exception("Amount is null.");
            }
            
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
            
            Transactions transaction = new Transactions()
            {
                
                TransactionId = new("00000000-0000-0000-0000-0000000" + u2),
                StartDate = DateTime.Now,
                EndDate = DateTime.Now.AddMonths(1),
                Reference = reference,
                Amount = amount,
                TenantId = tenantId,
                PaymentRequestId =  payId,
            };
            _tenantDb.Transactions.Add(transaction);
            _tenantDb.SaveChanges();
            return true;
           
        }
        
        // This function gets the transactions for a specific user.
        public List<Transactions> GetTransactions(string currentTenantId)
        {
            if (currentTenantId == null)
            {
                throw new Exception("Tenant id is null");
            }

            //string id = currentTenantId;
            var t = from transactions
                    in _tenantDb.Transactions
                where transactions.TenantId.Equals(currentTenantId)
                select transactions;

            var transactionList = t.ToList();
            return transactionList;
        }

        // This function gets the current subscription plan that the tenant is using.
        public Transactions GetPaidSubscription(string tenantId)
        {
            if (tenantId == null)
            {
                throw new Exception("TenantID is null");
            }

            Transactions getfirst;
       
            var t = from transactions
                    in _tenantDb.Transactions
                where transactions.TenantId.Equals(tenantId)
                select transactions;
            
             getfirst = t.OrderByDescending(x => x.StartDate)
                .FirstOrDefault();

                 return getfirst;
        }
        
        // This function is used to delete a transaction
        public bool DeleteTransaction(string id)
        {
            
            var transaction = _tenantDb.Transactions.Find(id);
            if (transaction == null)
            {
                return false;
            }
            
            
            _tenantDb.Transactions.Remove(transaction);
            _tenantDb.SaveChanges();
        
        
            
            
            return true;

        }
        
    }
}