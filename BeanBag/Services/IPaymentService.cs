using System.Collections.Generic;
using BeanBag.Models;

namespace BeanBag.Services
{
    // This is the interface for the payment service class.
    public interface IPaymentService
    {
        string ToUrlEncodedString(Dictionary<string, string> request);
        Dictionary<string, string> ToDictionary(string response);
        string GetMd5Hash(Dictionary<string, string> data, string encryptionKey);
        bool VerifyMd5Hash(Dictionary<string, string> data, string encryptionKey, string hash);
        public bool AddTransaction(string reference, string payId, string tenantId, double amount);
        List<Transactions> GetTransactions(string currentTenantId);
        public Transactions GetPaidSubscription(string tenantId);
        public bool DeleteTransaction(string id);
    }
}