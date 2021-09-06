using System.Collections.Generic;

namespace BeanBag.Services
{
    public interface IPaymentService
    {
        string ToUrlEncodedString(Dictionary<string, string> request);
        Dictionary<string, string> ToDictionary(string response);
     //   bool UpdateTransaction(Dictionary<string, string> request, string payRequestId);
       // Transaction GetTransaction(string payRequestId);
        string GetMd5Hash(Dictionary<string, string> data, string encryptionKey);
        bool VerifyMd5Hash(Dictionary<string, string> data, string encryptionKey, string hash);
        //void UpdateTransactionStatus(Transaction transaction);
        public bool AddTransaction(string reference, string payId, string tenantId, double amount);

    }
}