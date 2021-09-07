using System;
using System.ComponentModel.DataAnnotations;

namespace BeanBag.Models
{
    public class Transactions
    {
        [Key] 
        public Guid TransactionId { get; set; }
        public Guid TenantId { get; set; }
        public Guid PaymentRequestId { get; set; }
        public string Reference { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public float Amount { get; set; }
        
    }
}