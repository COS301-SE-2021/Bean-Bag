using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace BeanBag.Services
{
    public interface IPaymentService
    {
        
        string ToUrlEncodedString(Dictionary<string, string> request);
        Dictionary<string, string> ToDictionary(string response);
        bool AddTransaction(Dictionary<string, string> request, string payRequestId);
        bool UpdateTransaction(Dictionary<string, string> request, string payRequestId);
        DbLoggerCategory.Database.Transaction GetTransaction(string payRequestId);
        string GetMd5Hash(Dictionary<string, string> data, string encryptionKey);
        bool VerifyMd5Hash(Dictionary<string, string> data, string encryptionKey, string hash);
        ApplicationUser GetAuthenticatedUser();
        //void UpdateTransactionStatus(Transaction transaction);
    }
}