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
         private readonly TransactionDbContext _transactionDb;

         //Constructor sets database context
         public PaymentService(TransactionDbContext transactionDb)
         {
             _transactionDb = transactionDb;
         }

        #region Utilities
        
        // This function is used to Encode dictionary to Url string 
        public string ToUrlEncodedString(Dictionary<string, string> request)
        {
            StringBuilder builder = new StringBuilder();

            foreach (string key in request.Keys)
            {
                builder.Append("&");
                builder.Append(key);
                builder.Append("=");
                builder.Append(HttpUtility.UrlEncode(request[key]));
            }

            string result = builder.ToString().TrimStart('&');

            return result;
        }

        // This function converts a query string to dictionary 
        public Dictionary<string, string> ToDictionary(string response)
        {
            Dictionary<string, string> result = new Dictionary<string, string>();

            string[] valuePairs = response.Split('&');
            foreach (string valuePair in valuePairs)
            {
                string[] values = valuePair.Split('=');
                result.Add(values[0], HttpUtility.UrlDecode(values[1]));
            }

            return result;
        }
        #endregion Utility

        #region MD5 Hashing
        // Adapted from
        // https://msdn.microsoft.com/en-us/library/system.security.cryptography.md5(v=vs.110).aspx

        // This function computes a hashcode, this is for security purposes
        public string GetMd5Hash(Dictionary<string, string> data, string encryptionKey)
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

        // This function verifies that the encryption hashing took place 
        public bool VerifyMd5Hash(Dictionary<string, string> data, string encryptionKey, string hash)
        {
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
                return false;
            }
        }

    
        #endregion MD5 Hash

        
        #region Transactions 
        
        // This function adds a transaction to the transaction DB 
        public bool AddTransaction(Dictionary<string, string> request, string payRequestId)
        {
            try
            {
                Transaction transaction = new Transaction()
                {
                    DATE = DateTime.Now,
                    PAY_REQUEST_ID = payRequestId,
                    REFERENCE = request["REFERENCE"],
                    AMOUNT = int.Parse(request["AMOUNT"]),
                    CUSTOMER_EMAIL_ADDRESS = request["EMAIL"]
                };
                _transactionDb.Transaction.Add(transaction);
                _transactionDb.SaveChanges();
                return true;
            } catch (Exception )
            {
                // log somewhere
                // at least we tried
                return false;
            }
        }
        // get transaction using pay request Id
        public Transaction GetTransaction(string payRequestId)
        {
            var transaction = _transactionDb.Transaction.FirstOrDefault(p => p.PAY_REQUEST_ID == payRequestId);
            return transaction ?? new Transaction();

        }

        //This function updates a transaction in the transaction DB 
        public bool UpdateTransaction(Dictionary<string, string> request, string payRequestId)
        {
            bool isUpdated = false;

            Transaction transaction = GetTransaction(payRequestId);
            if (transaction == null)
                return false;

            transaction.TRANSACTION_STATUS = request["TRANSACTION_STATUS"];
            transaction.RESULT_DESC = request["RESULT_DESC"];
            try
            {
                _transactionDb.SaveChanges();
                isUpdated = true;
            }
            catch (Exception)
            {
                // ignored
            }

            return isUpdated;
        }

        #endregion Transaction
        
    }
}