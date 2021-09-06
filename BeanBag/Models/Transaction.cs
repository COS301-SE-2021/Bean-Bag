using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BeanBag.Models
{
    public class Transaction
    {
        [Key]
        public Guid TransactionId { get; set; }
        public Guid TenantId { get; set; }
        public string PaymentRequestId { get; set; }
        public int Reference { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public string Amount { get; set; }
        
    }
}